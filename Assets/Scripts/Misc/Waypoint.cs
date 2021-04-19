using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    // public ok here as is a data class
    public bool isExplored = false;
    public Waypoint exploredFrom;

    Vector2Int gridPos;

    const int gridSize = 1;

    void Awake()
    {
        this.SetGridPos();    
    }
    public int GetGridSize()
    {
        return gridSize;
    }

    public Vector2Int GetGridPos()
    {
        return new Vector2Int(
            Mathf.RoundToInt(transform.position.x / gridSize),
            Mathf.RoundToInt(transform.position.y / gridSize)
        );
    }

    void SetGridPos()
    {
        this.gridPos = GetGridPos();
    }
    public void ResetWaypointForPathfinding()
    {
        isExplored = false;
        exploredFrom = null;       
    }
}