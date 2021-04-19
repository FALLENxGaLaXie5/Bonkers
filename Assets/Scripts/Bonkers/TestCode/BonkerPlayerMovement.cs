using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonkerPlayerMovement : MonoBehaviour
{
    [SerializeField] float playerSpeed = 0.5f;
    [SerializeField] float currentSpeedCounter;
    [SerializeField] float bonkSpeed = 0.02f;
    Waypoint wayPoint;
    Vector2Int currentFacingDir = Vector2Int.down;

    //grid manager singleton dictionaries - references here for easy access in this class
    Dictionary<Vector2Int, Transform> grid;
    Dictionary<Vector2Int, Transform> occupiedTiles;
    // Start is called before the first frame update
    void Start()
    {
        wayPoint = GetComponent<Waypoint>();
        currentSpeedCounter = playerSpeed;
        grid = Grid.instance.grid;
        occupiedTiles = Grid.instance.occupiedTiles;
    }

    // Update is called once per frame
    void Update()
    {
        currentSpeedCounter += Time.deltaTime;
        Vector2Int previousPosition = wayPoint.GetGridPos();



        if (currentSpeedCounter <= playerSpeed) return;

        if (Input.GetKey(KeyCode.W) && IsMovementPossible(Vector2Int.up, previousPosition))
        {
            CalculateNewGridPos(Vector2Int.up);
        }
        else if (Input.GetKey(KeyCode.A) && IsMovementPossible(Vector2Int.left, previousPosition))
        {
            CalculateNewGridPos(Vector2Int.left);
        }
        else if (Input.GetKey(KeyCode.S) && IsMovementPossible(Vector2Int.down, previousPosition))
        {
            CalculateNewGridPos(Vector2Int.down);
        }
        else if (Input.GetKey(KeyCode.D) && IsMovementPossible(Vector2Int.right, previousPosition))
        {
            CalculateNewGridPos(Vector2Int.right);
        }

        //Input Action for moving a block
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!FacingCorrectDirection(previousPosition, currentFacingDir)) return;                        
            StartCoroutine(MoveOccupant(previousPosition, currentFacingDir));            
        }        
    }

    bool FacingCorrectDirection(Vector2Int prevPos, Vector2Int dir)
    {
        Vector2Int newPos = prevPos + dir;
        if (grid.ContainsKey(newPos) && grid[newPos].GetComponent<TileOccupation>().IsOccupied())
        {
            return true;
        }
        return false;
    }

    private void CalculateNewGridPos(Vector2Int dir)
    {
        currentSpeedCounter = 0;
        Vector2Int newGridPos = wayPoint.GetGridPos() + dir;
        SetNewGridPosition(newGridPos);
    }

    void SetFacingDirection(Vector2Int dir)
    {
        if (dir == Vector2Int.right)
        {
            currentFacingDir = Vector2Int.right;
            transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        else if (dir == Vector2Int.up)
        {
            currentFacingDir = Vector2Int.up;
            transform.rotation = Quaternion.Euler(0, 0, 180);
        }
        else if (dir == Vector2Int.left)
        {
            currentFacingDir = Vector2Int.left;
            transform.rotation = Quaternion.Euler(0, 0, 270);
        }
        else if (dir == Vector2Int.down)
        {
            currentFacingDir = Vector2Int.down;
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    bool IsMovementPossible(Vector2Int dir, Vector2Int currentPos)
    {
        SetFacingDirection(dir);
        Vector2Int newPos = wayPoint.GetGridPos() + dir;
        if (grid.ContainsKey(newPos) && !grid[newPos].GetComponent<TileOccupation>().IsOccupied())
        {
            return true;
        }
        return false;
    }

    IEnumerator MoveOccupant(Vector2Int playerPos, Vector2Int currentDir)
    {
        Vector2Int nextBlokPos = playerPos + (currentDir * 2);
        Vector2Int occupiedBlokPos = playerPos + currentDir;
        while (grid.ContainsKey(nextBlokPos) && !grid[nextBlokPos].GetComponent<TileOccupation>().IsOccupied())
        {
            occupiedTiles[occupiedBlokPos].position = grid[nextBlokPos].position;

            //set this position to unoccupied
            grid[occupiedBlokPos].GetComponent<TileOccupation>().SetOccupied(false);
            Transform occupantBlockTransform = occupiedTiles[occupiedBlokPos];
            occupiedTiles.Remove(occupiedBlokPos);
            occupiedTiles.Add(nextBlokPos, occupantBlockTransform);
            grid[nextBlokPos].GetComponent<TileOccupation>().SetOccupied(true);

            occupiedBlokPos = nextBlokPos;
            nextBlokPos = nextBlokPos + currentDir;

            //enter yield statement to make it look like its not just teleporting
            yield return new WaitForSeconds(bonkSpeed);
        }        
    }

    private void PrintDictionaries()
    {
        foreach (var item in grid)
        {
            print(item);
        }
        foreach (var item in occupiedTiles)
        {
            print (item);
        }
    }

    void SetNewGridPosition(Vector2Int newGridPos)
    {
        if (grid.ContainsKey(newGridPos))
        {
            transform.position = grid[newGridPos].position;
        }
    }
}
