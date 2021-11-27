using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bonkers.Core
{
    public class SpawnBlokControl : MonoBehaviour
    {
        #region Inspector and Public Variables
        [Header("The Turb Alliance")]
        [SerializeField] GameObject turbPrefab;
        [SerializeField] bool spawnTurbs = true;
        [Space(10)]

        [Header("The Grubber Alliance")]
        [SerializeField] GameObject grubberPrefab;
        [SerializeField] bool spawnGrubbers = true;
        [Space(10)]

        [Header("The Tar Alliance")]
        [SerializeField] GameObject tarSlimePrefab;
        [SerializeField] bool spawnTarSlimos = true;
        [Space(10)]

        [Header("The Toxic Alliance")]
        [SerializeField] GameObject toxicSlimePrefab;
        [SerializeField] bool spawnToxicSlimos = true;
        [Space(10)]

        [Header("The Ghostly Grubber Alliance")]
        [SerializeField] GameObject ghostlyGrubberPrefab;
        [SerializeField] bool spawnGhostlyGrubbers = true;
        [Space(10)]

        [SerializeField] LayerMask cannotSpawnMask = new LayerMask();

        [SerializeField] float minSpawnWaitTime = 2f;
        [SerializeField] float maxSpawnWaitTime = 20f;
        [SerializeField] int maxEnemies = 20;

        #endregion

        #region Class and Private Variables

        Transform turbAllianceTransform;
        Transform grubberAllianceTransform;
        Transform tarAllianceTransform;
        Transform toxicAllianceTransform;
        Transform ghostlyGrubberAllianceTransform;


        SpawnPoint[] spawnPoints;
        List<SpawnPoint> activeSpawnPoints = new List<SpawnPoint>();
        

        #endregion

        void Awake()
        {
            
            spawnPoints = GetComponentsInChildren<SpawnPoint>();
            foreach (SpawnPoint spawnPoint in spawnPoints)
            {
                if (spawnPoint.transform.gameObject.activeSelf)
                {
                    activeSpawnPoints.Add(spawnPoint);
                }
            }

            turbAllianceTransform = FindObjectOfType<TurbHolder>().transform;
            grubberAllianceTransform = FindObjectOfType<GrubberHolder>().transform;
            tarAllianceTransform = FindObjectOfType<SlimoHolder>().transform;
            toxicAllianceTransform = FindObjectOfType<ToxicSlimoHolder>().transform;
            ghostlyGrubberAllianceTransform = FindObjectOfType<GhostlyGrubberHolder>().transform;
        }

        void Start()
        {
            List<GameObject> lifeformPrefabs = new List<GameObject>();
            if (spawnTurbs) lifeformPrefabs.Add(turbPrefab);
            if (spawnTarSlimos) lifeformPrefabs.Add(tarSlimePrefab);
            if (spawnToxicSlimos) lifeformPrefabs.Add(toxicSlimePrefab);
            if (spawnGrubbers) lifeformPrefabs.Add(grubberPrefab);
            if (spawnGhostlyGrubbers) lifeformPrefabs.Add(ghostlyGrubberPrefab);

            foreach (SpawnPoint spawnPoint in activeSpawnPoints)
            {
                StartCoroutine(spawnPoint.SpawnLifeforms(minSpawnWaitTime, maxSpawnWaitTime, maxEnemies, lifeformPrefabs, cannotSpawnMask));
            }
            
        }
    }

}