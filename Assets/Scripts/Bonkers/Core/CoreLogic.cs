using Bonkers.BlokControl;
using Bonkers.Grid;
using Pathfinding;
using UnityEngine;

namespace Bonkers.Core
{
    public class CoreLogic : MonoBehaviour
    {
        [SerializeField] private Transform environment;
        [SerializeField] private Transform cameraTranform;
        [SerializeField] private AstarPath pathfinder;
        [SerializeField] private PatrolPoints patrolPoints;
        
        public void SetEnvironmentAsParent(Transform objectToChild)
        {
            objectToChild.parent = environment;
        }

        public void SetCameraPosition(float x, float y, float z) => cameraTranform.position = new Vector3(x, y, z);

        public void SetupNewPatrolPoints(int levelSize)
        {
            patrolPoints.RepopulateBackGroundTiles(levelSize);
        }

        public void ReferenceNewBlokSpawnSystem(BlokSpawnSystem blokSpawnSystem)
        {
            BlokSpawner blokSpawner = transform.GetComponentInChildren<BlokSpawner>();
            if (!blokSpawner)
            {
                Debug.Log("Could not find blok spawning component under core!");
                return;
            }

            blokSpawner.SetNewBlokSpawningSystem(blokSpawnSystem);
        }
        /// <summary>
        /// Sets the graph's center to the center of the map and scans the graph
        /// </summary>
        /// <param name="mapSize"></param>
        public void SetupPathfinder(int mapSize)
        {
            //GridGraph graph = pathfinder.data.gridGraph;
            //graph.center = new Vector3((float) mapSize / 2, (float) mapSize / 2, 0);
            //graph.Scan();
            /*
            GridGraph graph = pathfinder.data.AddGraph(typeof(GridGraph)) as GridGraph;
            graph.width = 100;
            graph.depth = 100;
            graph.nodeSize = 1;
            graph.center = new Vector3((float) mapSize / 2, (float) mapSize / 2, 0);
            */
        }
    }
}