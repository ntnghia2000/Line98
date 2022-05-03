using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private Transform cam;
    [SerializeField] Color WALKABLE_COLOR = new Color(196f/255f, 192f/255f, 189f/255f, 1.0f);
    [SerializeField] Color NON_WALKABLE_COLOR = new Color(0.1f/255f, 0.1f/255f, 0.1f/255f, 1.0f);
    private Tile[,] ListTile;
    // Start is called before the first frame update
    void Start()
    {
        ListTile = new Tile[width, height];
        GenerateGrid();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0)) {
            HandleToggleWalkable();
        }
    }

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

    private void ToggleWalkable(Tile tile) {
        tile.isWalkable = !tile.isWalkable;
        if(tile.isWalkable) {
            tile.SetInnerColor(WALKABLE_COLOR);
        } else {
            tile.SetInnerColor(NON_WALKABLE_COLOR);
        }
    }

    private void GenerateGrid() {
        for (int i = 0; i < width; i++) {
            for(int j = 0; j < height; j++) {
                var spawnTile = Instantiate(tilePrefab, new Vector3(i, j), Quaternion.identity);
                spawnTile.name = $"Tile {i} {j}";
                ListTile[i, j] = spawnTile.GetComponent<Tile>();

                var isOffset = (i % 2 == 0 && j % 2 != 0) || (i % 2 != 0 && j % 2 == 0);
                spawnTile.Init(isOffset);
            }
        }
        cam.transform.position = new Vector3((float) width/2 -0.5f, (float) height/2 -0.5f, -10f);
    }
}
