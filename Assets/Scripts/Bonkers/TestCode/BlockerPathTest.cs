using UnityEngine;
using System.Collections.Generic;
using Pathfinding;

public class BlockerPathTest : MonoBehaviour
{
    public BlockManager blockManager;
    public List<SingleNodeBlocker> obstacles;
    //public Transform target;

    BlockManager.TraversalProvider traversalProvider;


    


    public void Start()
    {
        // Create a traversal provider which says that a path should be blocked by all the SingleNodeBlockers in the obstacles array
        traversalProvider = new BlockManager.TraversalProvider(blockManager, BlockManager.BlockMode.OnlySelector, obstacles);
    }

    public void Update()
    {
        /**
        // Create a new Path object
        var path = ABPath.Construct(transform.position, target.position, null);

        // Make the path use a specific traversal provider
        path.traversalProvider = traversalProvider;

        // Calculate the path synchronously
        AstarPath.StartPath(path);
        path.BlockUntilCalculated();

        if (path.error)
        {
            Debug.Log("No path was found");
        }
        else
        {
            Debug.Log("A path was found with " + path.vectorPath.Count + " nodes");

            // Draw the path in the scene view
            for (int i = 0; i < path.vectorPath.Count - 1; i++)
            {
                Debug.DrawLine(path.vectorPath[i], path.vectorPath[i + 1], Color.red);
            }
        }
        */
    }

    public BlockManager.TraversalProvider GetTraversalProvider()
    {
        return this.traversalProvider;
    }


}