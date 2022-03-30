using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bonkers.Effects;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;

namespace Bonkers.BlokControl
{
    [CreateAssetMenu(fileName = "New Blok Spawning System", menuName = "Bloks/Create New Blok Spawning System")]
    public class BlokSpawnSystem : ScriptableObject
    {
        [SerializeField] private List<IndividualBlokPoolingData> possibleBloksToSpawn;
        
        [SerializeField] private Vector2Int minPosition = Vector2Int.zero;
        [SerializeField] private Vector2Int maxPosition = Vector2Int.zero;

        [SerializeField] private float minTimeBetweenSpawn = 5f;
        [SerializeField] private float maxTimeBetweenSpawn = 10f;

        public LayerMask wallLayers;
        public LayerMask enemyLayers;

        [SerializeField] private TweenEffect tweenEffect;

        public float MinTimeBetweenSpawns => minTimeBetweenSpawn;
        public float MaxTimeBetweenSpawns => maxTimeBetweenSpawn;
        public List<IndividualBlokPoolingData> PossibleBloksToSpawn => possibleBloksToSpawn;

        public IEnumerator SpawnBlok(GameObject blokToSpawn)
        {
            Vector2Int randomPosition;
            Collider2D blokCollider;
            do
            {
                randomPosition = new Vector2Int(Random.Range(minPosition.x, maxPosition.x), Random.Range(minPosition.y, maxPosition.y));
                blokCollider = Physics2D.OverlapCircle(randomPosition, 0.2f, wallLayers);
                yield return new WaitForSeconds(1f);
            } while (blokCollider);

            Vector2 spawnPosition = new Vector2(randomPosition.x, randomPosition.y);
            Collider2D enemyCheck = Physics2D.OverlapCircle(randomPosition, 0.7f, enemyLayers);

            blokToSpawn.transform.position = spawnPosition;
            blokToSpawn.transform.rotation = Quaternion.identity;
            blokToSpawn.SetActive(true);
            
            if (enemyCheck && enemyCheck.transform.tag == "Enemy")
            {
                if (blokToSpawn.TryGetComponent<BlokControl>(out BlokControl blokControl))
                {
                    blokControl.HitEnemy(enemyCheck.transform);
                }
            }
            
            if (tweenEffect) tweenEffect.ExecuteEffect(blokToSpawn.transform);
        }

        public IndividualBlokPoolingData GetRandomBlokTypeToSpawn()
        {
            return possibleBloksToSpawn[Random.Range(0, possibleBloksToSpawn.Count)];
        }
    }
}