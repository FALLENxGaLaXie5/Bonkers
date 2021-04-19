using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bonkers.Movement;

namespace Bonkers.Core
{
    public class SpawnSystem : MonoBehaviour
    {
        #region Inspector and Public Variables
        [Header("The Turb Alliance")]
        [SerializeField] GameObject turbPrefab;
        [SerializeField] bool spawnTurbs = true;
        [SerializeField] int maxTurbs = 10;
        [SerializeField] float minTurbSpawnWait = 5f;
        [SerializeField] float maxTurbSpawnWait = 50f;
        [Space(10)]
        
        [Header("The Grubber Alliance")]
        [SerializeField] GameObject grubberPrefab;
        [SerializeField] bool spawnGrubbers = true;
        [SerializeField] int maxGrubbers = 10;
        [SerializeField] float minGrubberSpawnWait = 5f;
        [SerializeField] float maxGrubberSpawnWait = 50f;
        [Space(10)]

        [Header("The Tar Alliance")]
        [SerializeField] GameObject tarSlimePrefab;
        [SerializeField] bool spawnTarSlimos = true;
        [SerializeField] int maxTarSlimos = 10;
        [SerializeField] float minTarSlimoSpawnWait = 5f;
        [SerializeField] float maxTarSlimoSpawnWait = 50f;
        [Space(10)]

        [Header("The Toxic Alliance")]
        [SerializeField] GameObject toxicSlimePrefab;
        [SerializeField] bool spawnToxicSlimos = true;
        [SerializeField] int maxToxicSlimos = 10;
        [SerializeField] float minToxicSlimoSpawnWait = 5f;
        [SerializeField] float maxToxicSlimoSpawnWait = 50f;
        [Space(10)]

        [Header("The Ghostly Grubber Alliance")]
        [SerializeField] GameObject ghostlyGrubberPrefab;
        [SerializeField] bool spawnGhostlyGrubbers = true;
        [SerializeField] int maxGhostlyGrubbers = 10;
        [SerializeField] float minGhostlyGrubberSpawnWait = 5f;
        [SerializeField] float maxGhostlyGrubberdSpawnWait = 50f;
        [Space(10)]

        [SerializeField] LayerMask cannotSpawnMask = new LayerMask();

        #endregion

        #region Class and Private Variables

        Transform turbAllianceTransform;
        Transform grubberAllianceTransform;
        Transform tarAllianceTransform;
        Transform toxicAllianceTransform;
        Transform ghostlyGrubberAllianceTransform;

        PatrolPoints spawnPoints;

        #endregion

        #region Unity Event Functions

        void Awake()
        {
            turbAllianceTransform = GetComponentInChildren<TurbHolder>().transform;
            grubberAllianceTransform = GetComponentInChildren<GrubberHolder>().transform;
            tarAllianceTransform = GetComponentInChildren<SlimoHolder>().transform;
            toxicAllianceTransform = GetComponentInChildren<ToxicSlimoHolder>().transform;
            ghostlyGrubberAllianceTransform = GetComponentInChildren<GhostlyGrubberHolder>().transform;
        }

        void Start()
        {
            CacheSpawnPoints();
            if (spawnTurbs) StartCoroutine(SpawnTurbs());
            if (spawnGrubbers) StartCoroutine(SpawnGrubbers());
            if (spawnTarSlimos) StartCoroutine(SpawnTarSlimos());
            if (spawnToxicSlimos) StartCoroutine(SpawnToxicSlimos());
            if (spawnGhostlyGrubbers) StartCoroutine(SpawnGhostlyGrubbers());
        }

        #endregion

        #region Class Functions

        private void CacheSpawnPoints()
        {
            spawnPoints = GameObject.FindGameObjectWithTag("Grid").transform.GetComponent<PatrolPoints>();
            if (!spawnPoints) Debug.LogError("PATROL POINTS WAS NOT FOUND IN SCENE: NEED TO ENSURE GRID OBJECT TAGGED AS 'Grid' IS IN SCENE");
        }

        /// <summary>
        /// Coroutine to spawn turbs on random interval between "minTurbSpawnWait" and "maxTurbSpawnWait". Spawns at random locations on map.
        /// </summary>
        /// <returns></returns>
        IEnumerator SpawnTurbs()
        {
            while(true)
            {
                yield return new WaitForSeconds(Random.Range(minTurbSpawnWait, maxTurbSpawnWait));

                if (turbAllianceTransform.childCount >= maxTurbs) continue;

                Transform possibleSpawnLocation = null;
                if (spawnPoints)
                {
                    possibleSpawnLocation = spawnPoints.patrolPoints[UnityEngine.Random.Range(0, spawnPoints.patrolPoints.Count)];
                    while (Physics2D.OverlapCircle(possibleSpawnLocation.position, 0.2f, cannotSpawnMask))
                    {
                        possibleSpawnLocation = spawnPoints.patrolPoints[UnityEngine.Random.Range(0, spawnPoints.patrolPoints.Count)];
                    }
                    GameObject newTurb = Instantiate(turbPrefab, possibleSpawnLocation.position, Quaternion.identity);
                    newTurb.transform.parent = turbAllianceTransform;
                }
                else
                {
                    Debug.LogError("WHERE IS PATROL POINTS OBJECT? NEED GRID OBJECT TAGGED 'Grid' WITH Patrol Points SCRIPT ON IT!");
                }
            }
        }

        IEnumerator SpawnGrubbers()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(minGrubberSpawnWait, maxGrubberSpawnWait));

                if (grubberAllianceTransform.childCount >= maxGrubbers) continue;

                Transform possibleSpawnLocation = null;
                if (spawnPoints)
                {
                    possibleSpawnLocation = spawnPoints.patrolPoints[UnityEngine.Random.Range(0, spawnPoints.patrolPoints.Count)];
                    while (Physics2D.OverlapCircle(possibleSpawnLocation.position, 0.2f, cannotSpawnMask))
                    {
                        possibleSpawnLocation = spawnPoints.patrolPoints[UnityEngine.Random.Range(0, spawnPoints.patrolPoints.Count)];
                    }
                    GameObject newGrubber = Instantiate(grubberPrefab, possibleSpawnLocation.position, Quaternion.identity);
                    newGrubber.transform.parent = grubberAllianceTransform;
                }
                else
                {
                    Debug.LogError("WHERE IS PATROL POINTS OBJECT? NEED GRID OBJECT TAGGED 'Grid' WITH Patrol Points SCRIPT ON IT!");
                }
            }
        }

        IEnumerator SpawnTarSlimos()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(minTarSlimoSpawnWait, maxTarSlimoSpawnWait));

                if (tarAllianceTransform.childCount >= maxTarSlimos) continue;

                Transform possibleSpawnLocation = null;
                if (spawnPoints)
                {
                    possibleSpawnLocation = spawnPoints.patrolPoints[UnityEngine.Random.Range(0, spawnPoints.patrolPoints.Count)];
                    while (Physics2D.OverlapCircle(possibleSpawnLocation.position, 0.2f, cannotSpawnMask))
                    {
                        possibleSpawnLocation = spawnPoints.patrolPoints[UnityEngine.Random.Range(0, spawnPoints.patrolPoints.Count)];
                    }
                    GameObject newTarSlimo = Instantiate(tarSlimePrefab, possibleSpawnLocation.position, Quaternion.identity);
                    newTarSlimo.transform.parent = tarAllianceTransform;
                }
                else
                {
                    Debug.LogError("WHERE IS PATROL POINTS OBJECT? NEED GRID OBJECT TAGGED 'Grid' WITH Patrol Points SCRIPT ON IT!");
                }
            }
        }

        IEnumerator SpawnToxicSlimos()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(minToxicSlimoSpawnWait, maxToxicSlimoSpawnWait));

                if (toxicAllianceTransform.childCount >= maxToxicSlimos) continue;

                Transform possibleSpawnLocation = null;
                if (spawnPoints)
                {
                    possibleSpawnLocation = spawnPoints.patrolPoints[UnityEngine.Random.Range(0, spawnPoints.patrolPoints.Count)];
                    while (Physics2D.OverlapCircle(possibleSpawnLocation.position, 0.2f, cannotSpawnMask))
                    {
                        possibleSpawnLocation = spawnPoints.patrolPoints[UnityEngine.Random.Range(0, spawnPoints.patrolPoints.Count)];
                    }
                    GameObject newToxicSlimo = Instantiate(toxicSlimePrefab, possibleSpawnLocation.position, Quaternion.identity);
                    newToxicSlimo.transform.parent = toxicAllianceTransform;
                }
                else
                {
                    Debug.LogError("WHERE IS PATROL POINTS OBJECT? NEED GRID OBJECT TAGGED 'Grid' WITH Patrol Points SCRIPT ON IT!");
                }
            }
        }

        IEnumerator SpawnGhostlyGrubbers()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(minGhostlyGrubberSpawnWait, maxGhostlyGrubberdSpawnWait));

                if (ghostlyGrubberAllianceTransform.childCount >= maxGhostlyGrubbers) continue;

                Transform possibleSpawnLocation = null;
                if (spawnPoints)
                {
                    possibleSpawnLocation = spawnPoints.patrolPoints[UnityEngine.Random.Range(0, spawnPoints.patrolPoints.Count)];
                    while (Physics2D.OverlapCircle(possibleSpawnLocation.position, 0.2f, cannotSpawnMask))
                    {
                        possibleSpawnLocation = spawnPoints.patrolPoints[UnityEngine.Random.Range(0, spawnPoints.patrolPoints.Count)];
                    }
                    GameObject newGhostlyGrubber = Instantiate(ghostlyGrubberPrefab, possibleSpawnLocation.position, Quaternion.identity);
                    newGhostlyGrubber.transform.parent = ghostlyGrubberAllianceTransform;
                }
                else
                {
                    Debug.LogError("WHERE IS PATROL POINTS OBJECT? NEED GRID OBJECT TAGGED 'Grid' WITH Patrol Points SCRIPT ON IT!");
                }
            }
        }

        public int GetMaxToxicSlimos()
        {
            return maxToxicSlimos;
        }

        #endregion

    }

}