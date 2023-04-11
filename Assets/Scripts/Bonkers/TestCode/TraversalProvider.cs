using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class TraversalProvider : MonoBehaviour
{

    public class Blocker : ITraversalProvider
    {
        public HashSet<GraphNode> blockedNodes = new HashSet<GraphNode>();

        public bool CanTraverse(Path path, GraphNode node)
        {
            // Override the default logic of which nodes can be traversed
            return DefaultITraversalProvider.CanTraverse(path, node) && !blockedNodes.Contains(node);
        }

        public uint GetTraversalCost(Path path, GraphNode node)
        {
            // Use the default costs
            return DefaultITraversalProvider.GetTraversalCost(path, node);
        }
    }

    public Blocker traversalProvider = new Blocker();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
