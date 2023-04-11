using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Random = UnityEngine.Random;

namespace  Bonkers.BlokControl
{
    [RequireComponent(typeof(BlokPool))]
    public class BlokSpawner : MonoBehaviour
    {
        [DetailedInfoBox("This System will spawn bloks in random locations within the system's min and max range from the Blok Pools.", "")]
        
        [SerializeField][InlineEditor] private BlokSpawnSystem spawnSystem;
        [SerializeField] private Transform activeBloksParent;
        
        private BlokPool blokPool;
        
        private void Awake()
        {
            blokPool = GetComponent<BlokPool>();
        }

        private void Start()
        {
            StartCoroutine(BeginSpawningBloks());
        }
        
        public IEnumerator BeginSpawningBloks()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(spawnSystem.MinTimeBetweenSpawns, spawnSystem.MaxTimeBetweenSpawns));

                GameObject blokToSpawn = blokPool.GetPooledBlokToSpawn(null);
                if(!blokToSpawn) continue;
                
                StartCoroutine(spawnSystem.SpawnBlok(blokToSpawn, activeBloksParent));
            }
        }

        /// <summary>
        /// Replaces the blok spawning system with a new one. Used in level generation to reference new map sizing for
        /// the blok spawning system.
        /// </summary>
        /// <param name="blokSpawnSystem"></param>
        public void SetNewBlokSpawningSystem(BlokSpawnSystem blokSpawnSystem) => spawnSystem = blokSpawnSystem;
    }
}