using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Bonkers.EnemySpawnManagement;
using Bonkers.Events;
using DG.Tweening;
using RoboRyanTron.Unite2017.Variables;
using Sirenix.OdinInspector;
using Random = UnityEngine.Random;

namespace Bonkers.BlokControl
{
    [RequireComponent(typeof(BlokAnimationControl))]
    public class SpawnBlokControl : MoveableBlokControl, ITrackableEventObject
    {
        #region Inspector and Public Variables

        [Serializable]
        public class PrefabSpawning
        {
            [SerializeField] private GameObject prefab;
            [SerializeField] private bool spawnPrefab;
            public GameObject Prefab => prefab;
            public bool SpawnPrefab => spawnPrefab;
        }

        [Header("The Turb Alliance")]
        [SerializeField] private PrefabSpawning turbPrefabSpawning;
        [Space(10)]
        
        [Header("The Grubber Alliance")]
        [SerializeField] private PrefabSpawning grubberPrefabSpawning;
        [Space(10)]
        
        [Header("The Tar Slimo Alliance")]
        [SerializeField] private PrefabSpawning tarSlimoPrefabSpawning;
        [Space(10)]
        
        [Header("The Toxic Slimo Alliance")]
        [SerializeField] private PrefabSpawning toxicSlimoPrefabSpawning;
        [Space(10)]
        
        [Header("The Ghostly Grubber Alliance")]
        [SerializeField] private PrefabSpawning ghostlyGrubberPrefabSpawning;
        [Space(10)]

        [SerializeField] LayerMask cannotSpawnMask = new LayerMask();

        [SerializeField] float minSpawnWaitTime = 2f;
        [SerializeField] float maxSpawnWaitTime = 20f;
        [SerializeField] int maxEnemies = 20;

        [ShowIf("@this.blokAnimationControl == null")] [SerializeField] private BlokAnimationControl blokAnimationControl;

        [SerializeField] private FloatReference rotateSpeed;

        #endregion

        #region Class and Private Variables

        Transform turbAllianceTransform;
        Transform grubberAllianceTransform;
        Transform tarAllianceTransform;
        Transform toxicAllianceTransform;
        Transform ghostlyGrubberAllianceTransform;


        SpawnPoint[] spawnPoints;
        List<SpawnPoint> activeSpawnPoints = new List<SpawnPoint>();


        private static int numActiveEnemies = 0;
        private static readonly List<Vector3> directions = new List<Vector3> {Vector3.up, Vector3.down, Vector3.left, Vector3.right};

        private static readonly Dictionary<Vector3, Vector3> directionsMapping = new Dictionary<Vector3, Vector3>
        {
            {Vector3.up, new Vector3(0, 0, 0)},
            {Vector3.down, new Vector3(0, 0, 180)},
            {Vector3.left, new Vector3(0, 0, 90)},
            {Vector3.right, new Vector3(0, 0, -90)}
        };

        private Vector3 spawnPosition = Vector3.zero;
        private Dictionary<PrefabSpawning, Transform> lifeformSpawningKey = new Dictionary<PrefabSpawning, Transform>();
        private List<PrefabSpawning> lifeformPrefabs = new List<PrefabSpawning>();
        private PrefabSpawning currentRandomPrefabSpawning = null;
        
        #endregion

        protected virtual void Start()
        {
            StartCoroutine(AttemptSpawnLifeforms());
        }
        
        /// <summary>
        /// Will be called by an event to assign these transforms
        /// </summary>
        /// <param name="holderParent"></param>
        public void AssignEnemyTransforms(GameObject holderParent)
        {
            turbAllianceTransform = holderParent.transform.GetComponentInChildren<TurbHolder>().transform;
            grubberAllianceTransform = holderParent.transform.GetComponentInChildren<GrubberHolder>().transform;
            tarAllianceTransform = holderParent.transform.GetComponentInChildren<SlimoHolder>().transform;
            toxicAllianceTransform = holderParent.transform.GetComponentInChildren<ToxicSlimoHolder>().transform;
            ghostlyGrubberAllianceTransform = holderParent.transform.GetComponentInChildren<GhostlyGrubberHolder>().transform;
            
            lifeformSpawningKey.Add(turbPrefabSpawning, turbAllianceTransform);
            lifeformSpawningKey.Add(grubberPrefabSpawning, grubberAllianceTransform);
            lifeformSpawningKey.Add(tarSlimoPrefabSpawning, tarAllianceTransform);
            lifeformSpawningKey.Add(toxicSlimoPrefabSpawning, toxicAllianceTransform);
            lifeformSpawningKey.Add(ghostlyGrubberPrefabSpawning, ghostlyGrubberAllianceTransform);
            lifeformPrefabs.AddRange(lifeformSpawningKey.Keys);
        }

        private IEnumerator AttemptSpawnLifeforms()
        {
            while (true)
            {
                if (!CheckIfAnyLifeformCanSpawn(lifeformPrefabs))
                {
                    //wait so no infinite loops if cannot spawn any lifeforms
                    yield return new WaitForSeconds(2f);
                }
                
                //choose random prefab- if it can't spawn, go back to top of spawning loop
                currentRandomPrefabSpawning = lifeformPrefabs[Random.Range(0, lifeformPrefabs.Count)];
                if (!currentRandomPrefabSpawning.SpawnPrefab) continue;
                
                yield return new WaitForSeconds(Random.Range(minSpawnWaitTime, maxSpawnWaitTime));

                if (numActiveEnemies >= maxEnemies) yield return null;

                int randomDirection = Random.Range(0, directions.Count - 1);
                spawnPosition = transform.position + directions[randomDirection];
                Collider2D blokCollider = Physics2D.OverlapCircle(spawnPosition, 0.2f, cannotSpawnMask);
                
                if(!blokCollider)
                {
                    //spawn random enemy here
                    Vector3 vector = SpawnBlokHelpers.FindClosestVector((spawnPosition - transform.position).normalized, directionsMapping.Keys.ToList());
                    transform.DORotate(directionsMapping[vector], rotateSpeed).onComplete = PlaySpawningAnimation;
                }
            }
        }

        private void PlaySpawningAnimation()
        {
            blokAnimationControl.PlayAnimation();
        }

        /// <summary>
        /// Called by animation event. Will actually spawn the object.
        /// </summary>
        private void SpawnLifeformObject()
        {
            if (spawnPosition == Vector3.zero) return;
            
            //Instantiate enemy object
            GameObject newLifeform = Instantiate(currentRandomPrefabSpawning.Prefab, spawnPosition, Quaternion.identity);
            //Assign new enemy to it's corresponding holder
            newLifeform.transform.parent = lifeformSpawningKey[currentRandomPrefabSpawning];
        }

        private bool CheckIfAnyLifeformCanSpawn(List<PrefabSpawning> prefabSpawnings)
        {
            foreach (var prefabSpawning in prefabSpawnings)
            {
                if (prefabSpawning.SpawnPrefab) return true;
            }
            return false;
        }
    }
}