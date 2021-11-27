using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace  Bonkers.BlokControl
{
    public class BlokSpawner : MonoBehaviour
    {
        [SerializeField] private BlokSpawnSystem spawnSystem;

        private void Start()
        {
            StartCoroutine(BeginSpawningBloks());
        }
        
        public IEnumerator BeginSpawningBloks()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(spawnSystem.MinTimeBetweenSpawns, spawnSystem.MaxTimeBetweenSpawns));
                StartCoroutine(spawnSystem.SpawnRandomBlok());
            }
        }
    }
}