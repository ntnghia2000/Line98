using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAI.PathFinder;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private Ball ball;
    [SerializeField] private Transform cam;
    [SerializeField] Color WALKABLE_COLOR = new Color(196f/255f, 192f/255f, 189f/255f, 1.0f);
    [SerializeField] Color NON_WALKABLE_COLOR = new Color(0.1f/255f, 0.1f/255f, 0.1f/255f, 1.0f);
    private Tile[,] GridTile;

    private GridTile mStartLocation;

    void Start()
    {
        GridTile = new Tile[width, height];
        GenerateGrid();
    }

    void Update()
    {
        //right mouse click
        if(Input.GetMouseButtonDown(1)) {
            HandleToggleWalkable();
        }
        //left mouse click
        if(Input.GetMouseButtonDown(0)) {
            HandleBallMove();
        }
    }

    private void HandleBallMove() {
        Vector2 rayPos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
        
        RaycastHit2D hit = Physics2D.Raycast(rayPos, Vector2.zero, 0.0f);
        if(hit) {
            GameObject hitObj = hit.transform.gameObject;
            Tile tile = hitObj.GetComponent<Tile>();
            if(tile) {
                // Vector3 ballPos = ball.GetBallPos();
                // Debug.Log(ballPos);
                // ball.AddWayPoint(tile.transform.position.x, tile.transform.position.y);
                StartCoroutine(Coroutine_FindPathAndMove(mStartLocation, tile.Tiles, ball));
            }
        }
    }

    IEnumerator Coroutine_FindPathAndMove(GridTile start, GridTile goal, Ball obj) {
        AStartPathFinder<Vector2Int> pathFinder = new AStartPathFinder<Vector2Int>();
        pathFinder.Heuristic = ManhattanCostFunc;
        pathFinder.NodeTraversalCost = EuclideanCostFunc;
        pathFinder.Initialize(start, goal);

        PathFinderStatus status = pathFinder.pStatus;
        while (status == PathFinderStatus.RUNNING) {
            pathFinder.Step();
            yield return null;
        }

        if( status == PathFinderStatus.SUCCESS) {
            List<Vector2Int> reverse_path = new List<Vector2Int>();
            PathFinding<Vector2Int>.PathFinderNode node = pathFinder.CurrentNode;
            
            while (node != null) {
                reverse_path.Add(node.Location.Value);
                node = node.Parent;
            }

            for (int i = reverse_path.Count - 1; i >= 0; i--) {
                Vector2Int index = reverse_path[i];
                Vector3 pos = GridTile[index.x, index.y].transform.position;
                obj.AddWayPoint(pos.x, pos.y);
            }
            mStartLocation = goal;
        }
        if (status == PathFinderStatus.FAILURE) {
            Debug.Log("Cannot find path");
        }
    }

    public static float ManhattanCostFunc(Vector2Int a, Vector2Int b) {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    public static float EuclideanCostFunc(Vector2Int a, Vector2Int b) {
        return (a-b).magnitude;
    }

    //handle tile walkable
    private void HandleToggleWalkable() {
        Vector2 rayPos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);

        RaycastHit2D hit = Physics2D.Raycast(rayPos, Vector2.zero, 0.0f);
        if(hit) {
            GameObject hitObj = hit.transform.gameObject;
            Tile tile = hitObj.GetComponent<Tile>();
            if(tile) {
                ToggleWalkable(tile);
            }
        }
    }

    //make tile walkable or not and change color
    private void ToggleWalkable(Tile tile) {
        tile.Tiles.isWalkable = !tile.Tiles.isWalkable;
        if(tile.Tiles.isWalkable) {
            tile.SetInnerColor(WALKABLE_COLOR);
        } else {
            tile.SetInnerColor(NON_WALKABLE_COLOR);
        }
    }

    private void GenerateGrid() {
        for (int i = 0; i < width; i++) {
            for(int j = 0; j < height; j++) {
                var spawnTile = Instantiate(tilePrefab, new Vector3(i, j, 0.0f), Quaternion.identity);
                spawnTile.name = $"Tile {i} {j}";
                GridTile[i, j] = spawnTile.GetComponent<Tile>();
                GridTile[i, j].Tiles = new GridTile(new Vector2Int(i, j), this);
            }
        }
        mStartLocation = GridTile[0, 0].Tiles;
        ball.transform.position = new Vector3(GridTile[0, 0].transform.position.x, GridTile[0, 0].transform.position.y, ball.transform.position.z);
        cam.transform.position = new Vector3((float) width/2 -0.5f, (float) height/2 -0.5f, -10f);
    }

    public List<Node<Vector2Int>> GetNeighbours(int xx, int yy) {
        List<Node<Vector2Int>> neighbours = new List<Node<Vector2Int>>();

        //top
        if(yy < height - 1) {
            int y = yy + 1;
            int x = xx;
            if(GridTile[x, y].Tiles.isWalkable) {
                neighbours.Add(GridTile[x, y].Tiles);
            }
        }
        //right
        if(xx < width - 1) {
            int y = yy;
            int x = xx + 1;
            if(GridTile[x, y].Tiles.isWalkable) {
                neighbours.Add(GridTile[x, y].Tiles);
            }
        }
        //bottom
        if(yy > 0) {
            int y = yy - 1;
            int x = xx;
            if(GridTile[x, y].Tiles.isWalkable) {
                neighbours.Add(GridTile[x, y].Tiles);
            }
        }
        //left
        if(xx > 0) {
            int y = yy;
            int x = xx - 1;
            if(GridTile[x, y].Tiles.isWalkable) {
                neighbours.Add(GridTile[x, y].Tiles);
            }
        }

        return neighbours;
    }
}
