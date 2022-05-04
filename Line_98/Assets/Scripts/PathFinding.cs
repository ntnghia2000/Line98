using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameAI {
    namespace PathFinder {
        public enum PathFinderStatus {
            NOT_INITALIZED,
            SUCCESS,
            FAILURE,
            RUNNING,
        }

        abstract public class Node<T> {
            public T Value { get; private set;}
            
            public Node (T value) {
                Value = value;
            }

            abstract public List<Node<T>> GetNeighbours();
        }

        abstract public class PathFinding<T> {
#region class PathFinderNode
            public class PathFinderNode {
                public PathFinderNode Parent { get; set;}
                public Node<T> Location { get; set;}
                
                // F = G + H
                public float Fcost { get; private set;}
                // H = heuristic cost to the destination/goal.
                public float Hcost { get; private set;}
                // G = the cost till this node from the start.
                public float Gcost { get; private set;}

                public PathFinderNode(Node<T> node, PathFinderNode parent, float G, float H) {
                    Location = node;
                    Parent = parent;
                    Hcost = H;
                    SetGCost(G);
                }

                public void SetGCost(float g) {
                    Gcost = g;
                    Fcost = Gcost + Hcost;
                }
            }
#endregion

#region cost functions related
            //the delegate for heurictic cost calculation
            public delegate float CostFunction(T a, T b);

            public CostFunction Heuristic;
            // the cost of traversing from one node to another
            public CostFunction NodeTraversalCost;
#endregion

#region open and close list
            protected List<PathFinderNode> openList = new List<PathFinderNode>();
            protected List<PathFinderNode> closeList = new List<PathFinderNode>();

            //search and return least Fcost value
            protected PathFinderNode GetLeastCostNode(List<PathFinderNode> list) {
                int best_index = 0;
                float best_cost = list[0].Fcost;

                for (int i = 1; i < list.Count; i++) {
                    if(best_cost > list[i].Fcost) {
                        best_cost = list[i].Fcost;
                        best_index = i;
                    }
                }

                return list[best_index];
            }

            //return index of tile that equal to list[best_index]
            protected int IsInList(List<PathFinderNode> list, T item) {
                for (int i = 0; i < list.Count; i++) {
                    if(EqualityComparer<T>.Default.Equals(list[i].Location.Value, item)) {
                        return i;
                    }
                }
                return -1;
            }
#endregion

#region delegate to handle state changes to PathFinderNode
            public delegate void DelegatePathFinderNode(PathFinderNode node);
            public DelegatePathFinderNode onChangeCurrentNode;
            public DelegatePathFinderNode onAddToOpenList;
            public DelegatePathFinderNode onAddToCloseList;
            public DelegatePathFinderNode onDestinationFound;
#endregion

            public Node<T> Start {get; private set;}
            public Node<T> Goal {get; private set;}
            protected PathFinderNode currentNode = null;

            public PathFinderNode CurrentNode {
                get {return currentNode;}
            }

            public void Reset() {
                if(pStatus == PathFinderStatus.RUNNING) {
                    return;
                }
                openList.Clear();
                closeList.Clear();
                pStatus = PathFinderStatus.NOT_INITALIZED;    
            }

            public bool Initialize (Node<T> start, Node<T> goal) {

                if(pStatus == PathFinderStatus.RUNNING) {
                    return false;
                }
                Reset();
                Start = start;
                Goal = goal;

                //calculate heuristic from start to goal
                float H = Heuristic(Start.Value, Goal.Value);

                PathFinderNode root = new PathFinderNode(Start, null, 0.0f, H);
                openList.Add(root);
                currentNode = root;
                onAddToOpenList?.Invoke(currentNode);
                onChangeCurrentNode?.Invoke(currentNode);
                pStatus = PathFinderStatus.RUNNING;
                return true;
            }

            public PathFinderStatus Step() {
                closeList.Add(currentNode);
                onAddToCloseList?.Invoke(currentNode);

                if(openList.Count == 0) {
                    pStatus = PathFinderStatus.FAILURE;

                    return pStatus;
                }

                currentNode = GetLeastCostNode(openList);
                onChangeCurrentNode?.Invoke(currentNode);
                openList.Remove(currentNode);

                //check if current node is goal
                if(EqualityComparer<T>.Default.Equals(currentNode.Location.Value, Goal.Value)) {
                    pStatus = PathFinderStatus.SUCCESS;
                    onDestinationFound?.Invoke(currentNode);
                    return pStatus;
                }

                List<Node<T>> neighbors = currentNode.Location.GetNeighbours();

                foreach (Node<T> tile in neighbors) {
                    AlgorithmSpecificImplementation(tile);
                }

                pStatus = PathFinderStatus.RUNNING;
                return pStatus;
            }

            abstract protected void AlgorithmSpecificImplementation(Node<T> tile);

            public PathFinderStatus pStatus {
                get;
                private set;
            }
        }

        public class AStartPathFinder<T> : PathFinding<T> {
            protected override void AlgorithmSpecificImplementation(Node<T> tile)
            {
                if(IsInList(closeList, tile.Value) == -1) {
                    float G = currentNode.Gcost + NodeTraversalCost(currentNode.Location.Value, tile.Value);
                    float H = Heuristic(tile.Value, Goal.Value);

                    int idOList = IsInList(openList, tile.Value);
                    if(idOList == -1) {
                        PathFinderNode n = new PathFinderNode(tile, currentNode, G, H); 
                        openList.Add(n);
                        onAddToOpenList?.Invoke(n);
                    } else {
                        float oldG = openList[idOList].Gcost;
                        if(G < oldG) {
                            openList[idOList].SetGCost(G);
                            openList[idOList].Parent = currentNode;
                            onAddToOpenList?.Invoke(openList[idOList]);
                        }
                    }
                }
            }
        }
    }
}
