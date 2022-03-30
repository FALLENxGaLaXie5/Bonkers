using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace  Bonkers.BlokControl
{
    [RequireComponent(typeof(BlokPool))]
    public class BlokSpawner : MonoBehaviour
    {
        [SerializeField] private BlokSpawnSystem spawnSystem;

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
                
                StartCoroutine(spawnSystem.SpawnBlok(blokToSpawn));
            }
        }
    }
}