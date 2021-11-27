using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bonkers.Movement;
using Sirenix.OdinInspector;

namespace Bonkers.Core
{
    public class SpawnSystem : MonoBehaviour
    {
        #region Inspector and Public Variables
        [TitleGroup("The Turb Alliance", Alignment = TitleAlignments.Left)]
        [HorizontalGroup("The Turb Alliance/Base", LabelWidth = 80)]
        [SerializeField][Required][PreviewField(60, Alignment = ObjectFieldAlignment.Left)][BoxGroup("The Turb Alliance/Base/Left", false)][LabelWidth(140)] GameObject turbPrefab;
        [SerializeField] [BoxGroup("The Turb Alliance/Base/Configuration")][LabelWidth(140)] bool spawnTurbs = true;
        [SerializeField] [BoxGroup("The Turb Alliance/Base/Configuration")] [LabelWidth(140)] int maxTurbs = 10;
        [SerializeField] [BoxGroup("The Turb Alliance/Base/Configuration")] [LabelWidth(140)] float minTurbSpawnWait = 5f;
        [SerializeField] [BoxGroup("The Turb Alliance/Base/Configuration")] [LabelWidth(140)] float maxTurbSpawnWait = 50f;
        [Space(10)]

        [TitleGroup("The Grubber Alliance", Alignment = TitleAlignments.Left)]
        [HorizontalGroup("The Grubber Alliance/Base", LabelWidth = 80)]
        [SerializeField] [Required] [PreviewField(60, Alignment = ObjectFieldAlignment.Left)] [BoxGroup("The Grubber Alliance/Base/Left", false)] [LabelWidth(140)] GameObject grubberPrefab;
        [SerializeField] [BoxGroup("The Grubber Alliance/Base/Configuration")] [LabelWidth(140)] bool spawnGrubbers = true;
        [SerializeField] [BoxGroup("The Grubber Alliance/Base/Configuration")] [LabelWidth(140)] int maxGrubbers = 10;
        [SerializeField] [BoxGroup("The Grubber Alliance/Base/Configuration")] [LabelWidth(140)] float minGrubberSpawnWait = 5f;
        [SerializeField] [BoxGroup("The Grubber Alliance/Base/Configuration")] [LabelWidth(140)] float maxGrubberSpawnWait = 50f;
        [Space(10)]

        [TitleGroup("The Tar Alliance", Alignment = TitleAlignments.Left)]
        [HorizontalGroup("The Tar Alliance/Base", LabelWidth = 80)]
        [SerializeField] [Required] [PreviewField(60, Alignment = ObjectFieldAlignment.Left)] [BoxGroup("The Tar Alliance/Base/Left", false)] [LabelWidth(140)] GameObject tarSlimePrefab;
        [SerializeField] [BoxGroup("The Tar Alliance/Base/Configuration")] [LabelWidth(140)] bool spawnTarSlimos = true;
        [SerializeField] [BoxGroup("The Tar Alliance/Base/Configuration")] [LabelWidth(140)] int maxTarSlimos = 10;
        [SerializeField] [BoxGroup("The Tar Alliance/Base/Configuration")] [LabelWidth(140)] float minTarSlimoSpawnWait = 5f;
        [SerializeField] [BoxGroup("The Tar Alliance/Base/Configuration")] [LabelWidth(140)] float maxTarSlimoSpawnWait = 50f;
        [Space(10)]

        [TitleGroup("The Toxic Alliance", Alignment = TitleAlignments.Left)]
        [HorizontalGroup("The Toxic Alliance/Base", LabelWidth = 80)]
        [SerializeField] [Required] [PreviewField(60, Alignment = ObjectFieldAlignment.Left)] [BoxGroup("The Toxic Alliance/Base/Left", false)] [LabelWidth(140)] GameObject toxicSlimePrefab;
        [SerializeField] [BoxGroup("The Toxic Alliance/Base/Configuration")] [LabelWidth(140)] bool spawnToxicSlimos = true;
        [SerializeField] [BoxGroup("The Toxic Alliance/Base/Configuration")] [LabelWidth(140)] int maxToxicSlimos = 10;
        [SerializeField] [BoxGroup("The Toxic Alliance/Base/Configuration")] [LabelWidth(140)] float minToxicSlimoSpawnWait = 5f;
        [SerializeField] [BoxGroup("The Toxic Alliance/Base/Configuration")] [LabelWidth(140)] float maxToxicSlimoSpawnWait = 50f;
        [Space(10)]

        [TitleGroup("The Ghostly Grubber Alliance", Alignment = TitleAlignments.Left)]
        [HorizontalGroup("The Ghostly Grubber Alliance/Base", LabelWidth = 80)]
        [SerializeField] [Required] [PreviewField(60, Alignment = ObjectFieldAlignment.Left)] [BoxGroup("The Ghostly Grubber Alliance/Base/Left", false)] [LabelWidth(140)] GameObject ghostlyGrubberPrefab;
        [SerializeField] [BoxGroup("The Ghostly Grubber Alliance/Base/Configuration")] [LabelWidth(140)] bool spawnGhostlyGrubbers = true;
        [SerializeField] [BoxGroup("The Ghostly Grubber Alliance/Base/Configuration")] [LabelWidth(140)] int maxGhostlyGrubbers = 10;
        [SerializeField] [BoxGroup("The Ghostly Grubber Alliance/Base/Configuration")] [LabelWidth(140)] float minGhostlyGrubberSpawnWait = 5f;
        [SerializeField] [BoxGroup("The Ghostly Grubber Alliance/Base/Configuration")] [LabelWidth(140)] float maxGhostlyGrubberdSpawnWait = 50f;
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
            if (spawnTurbs) StartCoroutine(SpawnLifeforms(turbPrefab, minTurbSpawnWait, maxTurbSpawnWait, maxTurbs, turbAllianceTransform));
            if (spawnGrubbers) StartCoroutine(SpawnLifeforms(grubberPrefab, minGrubberSpawnWait, maxGrubberSpawnWait, maxGrubbers, grubberAllianceTransform));
            if (spawnTarSlimos) StartCoroutine(SpawnLifeforms(tarSlimePrefab, minTarSlimoSpawnWait, maxTarSlimoSpawnWait, maxTarSlimos, tarAllianceTransform));
            if (spawnToxicSlimos) StartCoroutine(SpawnLifeforms(toxicSlimePrefab, minToxicSlimoSpawnWait, maxToxicSlimoSpawnWait, maxToxicSlimos, toxicAllianceTransform));
            if (spawnGhostlyGrubbers) StartCoroutine(SpawnLifeforms(ghostlyGrubberPrefab, minGhostlyGrubberSpawnWait, maxGhostlyGrubberdSpawnWait, maxGhostlyGrubbers, ghostlyGrubberAllianceTransform));
        }

        #endregion

        #region Class Functions

        void CacheSpawnPoints()
        {
            spawnPoints = GameObject.FindGameObjectWithTag("Grid").transform.GetComponent<PatrolPoints>();
            if (!spawnPoints) Debug.LogError("PATROL POINTS WAS NOT FOUND IN SCENE: NEED TO ENSURE GRID OBJECT TAGGED AS 'Grid' IS IN SCENE");
        }

        /// <summary>
        /// Coroutine to spawn turbs on random interval between "minTurbSpawnWait" and "maxTurbSpawnWait". Spawns at random locations on map.
        /// </summary>
        /// <returns></returns>
        IEnumerator SpawnLifeforms(GameObject prefab, float minSpawnWait, float maxSpawnWait, float maxLifeforms, Transform objectHolder)
        {
            while(true)
            {
                yield return new WaitForSeconds(Random.Range(minSpawnWait, maxSpawnWait));

                if (objectHolder.childCount >= maxLifeforms) continue;

                Transform possibleSpawnLocation = null;
                if (spawnPoints)
                {
                    possibleSpawnLocation = spawnPoints.patrolPoints[UnityEngine.Random.Range(0, spawnPoints.patrolPoints.Count)];
                    while (Physics2D.OverlapCircle(possibleSpawnLocation.position, 0.2f, cannotSpawnMask))
                    {
                        possibleSpawnLocation = spawnPoints.patrolPoints[UnityEngine.Random.Range(0, spawnPoints.patrolPoints.Count)];
                    }
                    GameObject newLifeform = Instantiate(prefab, possibleSpawnLocation.position, Quaternion.identity);
                    newLifeform.transform.parent = objectHolder;
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