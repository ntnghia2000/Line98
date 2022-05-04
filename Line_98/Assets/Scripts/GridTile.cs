using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAI.PathFinder;

public class GridTile : Node<Vector2Int>
{
    public bool isWalkable = true;
    private GridManager mGrid;
    public GridTile(Vector2Int index, GridManager grid) : base(index) {
        mGrid = grid;
    }
    public override List<Node<Vector2Int>> GetNeighbours() {
        return mGrid.GetNeighbours(Value.x, Value.y);
    }
}
