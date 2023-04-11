using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteAlways]
public class Grid : MonoBehaviour
{
    public static Grid instance = null;

    public Dictionary<Vector2Int, Transform> grid = new Dictionary<Vector2Int, Transform>();
    public Dictionary<Vector2Int, Transform> occupiedTiles = new Dictionary<Vector2Int, Transform>();

    public List<Vector2Int> gridPositions = new List<Vector2Int>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(gameObject);
        }

        LoadBlocksIntoGrid();

    }

    void Start()
    {
    }

    private void LoadBlocksIntoGrid()
    {
        var tiles = GameObject.FindGameObjectsWithTag("MainTile");
        var occupiedTileWalls = GameObject.FindGameObjectsWithTag("MoveableWall");

        foreach (GameObject tile in occupiedTileWalls)
        {
            Waypoint waypoint = tile.GetComponent<Waypoint>();
            var gridPos = waypoint.GetGridPos();

            if (occupiedTiles.ContainsKey(gridPos))
            {
                Debug.LogWarning("Skipping overlapping block " + tile.GetComponent<Waypoint>());
            }
            else
            {
                occupiedTiles.Add(gridPos, tile.transform);
            }
        }


        foreach (GameObject tile in tiles)
        {
            Waypoint waypoint = tile.GetComponent<Waypoint>();
            var gridPos = waypoint.GetGridPos();
            if (grid.ContainsKey(gridPos))
            {
                Debug.LogWarning("Skipping overlapping block " + tile.GetComponent<Waypoint>());
            }
            else
            {
                if (occupiedTiles.ContainsKey(gridPos))
                {
                    tile.GetComponent<TileOccupation>().SetOccupied(true);
                }
                grid.Add(gridPos, tile.transform);
                gridPositions.Add(gridPos);
                tile.GetComponentInChildren<TextMesh>().text = gridPos.x + ", " + gridPos.y;
            }
        }

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Dictionary<Vector2Int, Transform> GetGrid()
    {
        return this.grid;
    }

    public Dictionary<Vector2Int, Transform> GetOccupiedTiles()
    {
        return this.occupiedTiles;
    }
}
