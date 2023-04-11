using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bonkers.EnemySpawnManagement
{
    public class SpawnPoint : MonoBehaviour
    {
        int currentSpawned = 0;


        public IEnumerator SpawnLifeforms(float minSpawnWait, float maxSpawnWait, int maxEnemies, List<GameObject> lifeformPrefabs, LayerMask cannotSpawnMask)
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(minSpawnWait, maxSpawnWait));

                if (currentSpawned >= maxEnemies) yield return null;

                Collider2D blokCollider = Physics2D.OverlapCircle(transform.position, 0.2f, cannotSpawnMask);
                
                if(!blokCollider)
                {
                    //spawn random enemy here
                    GameObject newLifeform = Instantiate(lifeformPrefabs[Random.Range(0, lifeformPrefabs.Count)], transform.position, Quaternion.identity);
                }
                else
                {
                    continue;
                }
                
            }
        }
    }
}

