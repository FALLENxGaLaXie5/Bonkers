using System.Collections;
using UnityEngine;
//using Bonkers.Movement;
using Bonkers.Grid;
using CodeMonkey.Utils;
using Sirenix.OdinInspector;

namespace Bonkers.EnemySpawnManagement
{
    [CreateAssetMenu(fileName = "Enemy Spawn System", menuName = "Spawn Systems/Enemy Spawn System", order = 1)]
    public class EnemySpawnSystem : ScriptableObject
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

        #region Data Access
        //Turb Data Access
        public GameObject TurbPrefab => turbPrefab;
        public bool SpawnTurbs => spawnTurbs;
        public int MaxTurbs => maxTurbs;
        public float MinTurbSpawnWait => minTurbSpawnWait;
        public float MaxTurbSpawnWait => maxTurbSpawnWait;
        
        //Grubber Data Access
        public GameObject GrubberPrefab => grubberPrefab;
        public bool SpawnGrubbers => spawnGrubbers;
        public int MaxGrubbers => maxGrubbers;
        public float MinGrubberSpawnWait => minGrubberSpawnWait;
        public float MaxGrubberSpawnWait => maxGrubberSpawnWait;
        
        //Tar Slimos Data Access
        public GameObject TarSlimoPrefab => tarSlimePrefab;
        public bool SpawnTarSlimos => spawnTarSlimos;
        public int MaxTarSlimos => maxTarSlimos;
        public float MinTarSlimoSpawnWait => minTarSlimoSpawnWait;
        public float MaxTarSlimoSpawnWait => maxTarSlimoSpawnWait;
        
        //Toxic Slimos Data Access
        public GameObject ToxicSlimoPrefab => toxicSlimePrefab;
        public bool SpawnToxicSlimos => spawnToxicSlimos;
        public int MaxToxicSlimos => maxToxicSlimos;
        public float MinToxicSlimoSpawnWait => minToxicSlimoSpawnWait;
        public float MaxToxicSlimoSpawnWait => maxToxicSlimoSpawnWait;
        
        //Ghostly Grubber Data Access
        public GameObject GhostlyGrubberPrefab => ghostlyGrubberPrefab;
        public bool SpawnGhostlyGrubbers => spawnGhostlyGrubbers;
        public int MaxGhostlyGrubbers => maxGhostlyGrubbers;
        public float MinGhostlyGrubberSpawnWait => minGhostlyGrubberSpawnWait;
        public float MaxGhostlyGrubberSpawnWait => maxGhostlyGrubberdSpawnWait;
        
        #endregion

        #region Class and Private Variables

        PatrolPoints spawnPoints;

        #endregion

        #region Unity Event Functions

        public void InitializeSystem()
        {
            CacheSpawnPoints();
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
        public IEnumerator SpawnLifeforms(GameObject prefab, float minSpawnWait, float maxSpawnWait, float maxLifeforms, EnemyHolder objectHolder, bool spawnLifeform = true)
        {
            if (!spawnLifeform)
            {
                yield break;
            }
            
            float timeForNextLifeform = 0f;
            while(true)
            {
                yield return new WaitForSeconds(Random.Range(minSpawnWait, maxSpawnWait));
                if (objectHolder.transform.childCount >= maxLifeforms)
                {
                    continue;
                }
                SpawnLifeform(prefab, objectHolder);
                //TODO: Need to calculate an actual wait time for each of these timers it will create
                //FunctionTimer.Create(() => SpawnLifeform(prefab, objectHolder), Random.Range(minSpawnWait, maxSpawnWait));
            }
        }
        

        private void SpawnLifeform(GameObject prefab, EnemyHolder objectHolder)
        {
            Transform possibleSpawnLocation = null;
            if (spawnPoints)
            {
                possibleSpawnLocation = spawnPoints.patrolPoints[Random.Range(0, spawnPoints.patrolPoints.Count)];
                while (Physics2D.OverlapCircle(possibleSpawnLocation.position, 0.2f, cannotSpawnMask))
                {
                    possibleSpawnLocation =
                        spawnPoints.patrolPoints[UnityEngine.Random.Range(0, spawnPoints.patrolPoints.Count)];
                }

                GameObject newLifeform = Instantiate(prefab, possibleSpawnLocation.position, Quaternion.identity);
                newLifeform.transform.parent = objectHolder.transform;
            }
            else
            {
                Debug.LogError(
                    "WHERE IS PATROL POINTS OBJECT? NEED GRID OBJECT TAGGED 'Grid' WITH Patrol Points SCRIPT ON IT!");
            }
        }

        #endregion

    }

}