using UnityEngine;

namespace Bonkers.EnemySpawnManagement
{
    public class ToxicSlimoHolder : EnemyHolder, IEnemyHolder
    {
        [SerializeField] GameObject toxicSlimoPrefab;
        [SerializeField] private EnemySpawnSystem enemySpawnSystem;
        
        public void AttemptSpawn(Vector3 spawnPosition)
        {
            if (enemySpawnSystem == null) return;
            if (transform.childCount >= enemySpawnSystem.MaxToxicSlimos + 5) return;
            
            GameObject newToxicSlimo = Instantiate(toxicSlimoPrefab, spawnPosition, Quaternion.identity);
            newToxicSlimo.transform.parent = transform;
        }
    }
}