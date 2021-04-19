using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingSearch : MonoBehaviour
{

    public Waypoint startWaypoint, endWaypoint;

    Waypoint aiWaypoint;

    //Dictionary<Vector2Int, Waypoint> grid = new Dictionary<Vector2Int, Waypoint>();
    Dictionary<Vector2Int, Transform> occupiedTiles;
    List<Vector2Int> gridPositions;

    Dictionary<Vector2Int, Transform> grid;



    Queue<Waypoint> queue = new Queue<Waypoint>();
    bool isRunning = true;
    Waypoint searchCenter;
    public List<Waypoint> path = new List<Waypoint>();
    List<Waypoint> resetWaypointsList = new List<Waypoint>();

    Vector2Int[] directions = {
        Vector2Int.up,
        Vector2Int.right,
        Vector2Int.down,
        Vector2Int.left
    };


    private void Awake()
    {
    }
    void Start()
    {
        grid = Grid.instance.grid;
        occupiedTiles = Grid.instance.occupiedTiles;
        gridPositions = Grid.instance.gridPositions;
        aiWaypoint = GetComponent<Waypoint>();
    }



    public void ResetPath()
    {
        path.Clear();
        queue.Clear();
        isRunning = true;
        searchCenter = null;
        foreach  (Waypoint waypoint in resetWaypointsList)
        {
            waypoint.ResetWaypointForPathfinding();
        }
        resetWaypointsList.Clear();
    }

    public List<Waypoint> GetPath()
    {
        if (path.Count == 0)
        {
            CalculatePath();
        }
        return path;
    }

    private void CalculatePath()
    {
        //LoadBlocks();
        StartCoroutine(BreadthFirstSearch());
        CreatePath();
    }

    private void CreatePath()
    {
        SetAsPath(endWaypoint);

        Waypoint previous = endWaypoint.exploredFrom;

        while (previous != startWaypoint)
        {
            //For null errors where previous is somehow not getting set to something            
            SetAsPath(previous);
            previous = previous.exploredFrom;
        }

        SetAsPath(startWaypoint);
        path.Reverse();
    }

    private void SetAsPath(Waypoint waypoint)
    {
        path.Add(waypoint);
    }

    private IEnumerator BreadthFirstSearch()
    {
        queue.Enqueue(startWaypoint);
        resetWaypointsList.Add(startWaypoint);


        int loopCounter = 0;
        //possible fix
        while (loopCounter < 700 && queue.Count > 0 && isRunning)
        {
            searchCenter = queue.Dequeue();
            HaltIfEndFound();
            ExploreNeighbours();
            searchCenter.isExplored = true;


            searchCenter.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
            yield return new WaitForSeconds(0.02f);
            loopCounter++;
        }
        Debug.LogWarning("Hit loop " + loopCounter + "!");
    }

    private void HaltIfEndFound()
    {
        if (searchCenter.GetGridPos() == endWaypoint.GetGridPos())
        {
            resetWaypointsList.Add(endWaypoint);
            isRunning = false;
        }
    }

    private void ExploreNeighbours()
    {
        if (!isRunning) { return; }

        foreach (Vector2Int direction in directions)
        {
            Vector2Int neighbourCoordinates = searchCenter.GetGridPos() + direction;
            if (grid.ContainsKey(neighbourCoordinates) && !grid[neighbourCoordinates].GetComponent<TileOccupation>().IsOccupied())
            {
                QueueNewNeighbours(neighbourCoordinates);
            }
        }
    }

    private void QueueNewNeighbours(Vector2Int neighbourCoordinates)
    {
        Waypoint neighbour = grid[neighbourCoordinates].GetComponent<Waypoint>();
        if (neighbour.isExplored || queue.Contains(neighbour))
        {
            // do nothing
        }
        else
        {
            queue.Enqueue(neighbour);
            resetWaypointsList.Add(neighbour);
            neighbour.exploredFrom = searchCenter;
        }
    }

    public void SetStartWaypoint()
    {
        this.startWaypoint = grid[aiWaypoint.GetGridPos()].gameObject.GetComponent<Waypoint>();
    }

    public void SetEndWaypoint()
    {
        Vector2Int randomPos = gridPositions[UnityEngine.Random.Range(0, grid.Count)];
        this.endWaypoint = grid[randomPos].gameObject.GetComponent<Waypoint>();
        while (grid[randomPos].GetComponent<TileOccupation>().IsOccupied())
        {
            path = new List<Waypoint>();
        }
    }

    public List<Waypoint> SetNewRandomPath()
    {
        SetStartWaypoint();
        SetEndWaypoint();
        List<Waypoint> path = GetPath();
        List<Waypoint> pathToFollow = new List<Waypoint>(path);
        ResetPath();
        return pathToFollow;
    }

    private void LoadBlocks()
    {
        var waypoints = FindObjectsOfType<Waypoint>();

        foreach (Waypoint waypoint in waypoints)
        {
            var gridPos = waypoint.GetGridPos();
            if (grid.ContainsKey(gridPos))
            {
                Debug.LogWarning("Skipping overlapping block " + waypoint);
            }
            else
            {
                grid.Add(gridPos, waypoint.gameObject.transform);
            }
        }
    }
}