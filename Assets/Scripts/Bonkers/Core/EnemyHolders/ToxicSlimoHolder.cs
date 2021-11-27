using UnityEngine;

namespace Bonkers.Core
{
    public class ToxicSlimoHolder : MonoBehaviour, IEnemyHolder
    {
        [SerializeField] GameObject toxicSlimoPrefab;
        public void AttemptSpawn(Vector3 spawnPosition)
        {
            if (transform.childCount >= FindObjectOfType<SpawnSystem>().GetMaxToxicSlimos() + 5) return;
            
            GameObject newToxicSlimo = Instantiate(toxicSlimoPrefab, spawnPosition, Quaternion.identity);
            newToxicSlimo.transform.parent = transform;
        }
    }
}