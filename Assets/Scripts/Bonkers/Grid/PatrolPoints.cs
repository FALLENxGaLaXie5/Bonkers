using System.Collections.Generic;
using UnityEngine;
using Bonkers.Extensions;
using UnityEditor;

namespace Bonkers.Grid
{
    public class PatrolPoints : MonoBehaviour
    {
        [SerializeField] private GameObject backgroundTilePrefab;
        [SerializeField] private Transform backgroundTilesParent;
        public List<Transform> patrolPoints;

        private void ClearBackgroundTiles()
        {
            patrolPoints.Clear();
            backgroundTilesParent.SetChildrenInactiveImmediate();
        }

        public void RepopulateBackGroundTiles(int levelSize)
        {
            #if UNITY_EDITOR
            ClearBackgroundTiles();
            for (int r = 1; r < levelSize - 1; r++)
            {
                for (int c = 1; c < levelSize - 1; c++)
                {
                    GameObject tile = PrefabUtility.InstantiatePrefab(backgroundTilePrefab) as GameObject;
                    tile.transform.parent = backgroundTilesParent;
                    tile.transform.position = new Vector3(r, c, 0);
                    tile.GetComponent<SpriteRenderer>().enabled = false;
                    patrolPoints.Add(tile.transform);
                }
            }   
            #endif
        }
    }
}
