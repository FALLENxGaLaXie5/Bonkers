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

        #region Properties
        
        public float MinTimeBetweenSpawns => minTimeBetweenSpawn;
        public float MaxTimeBetweenSpawns => maxTimeBetweenSpawn;
        public List<IndividualBlokPoolingData> PossibleBloksToSpawn => possibleBloksToSpawn;
        public LayerMask WallLayers => wallLayers;
        public LayerMask EnemyLayers => enemyLayers;
        public Vector2Int MinPosition => minPosition;
        public Vector2Int MaxPosition => maxPosition;
        public TweenEffect TweenEffect => tweenEffect;

        #endregion
        
        /// <summary>
        /// Public access to create instances of this scriptable object. Internally, initializes everything needed for the data
        /// </summary>
        /// <param name="possibleBloksToSpawn"></param>
        /// <param name="minPosition"></param>
        /// <param name="maxPosition"></param>
        /// <param name="minTimeBetweenSpawn"></param>
        /// <param name="maxTimeBetweenSpawn"></param>
        /// <param name="wallLayers"></param>
        /// <param name="enemyLayers"></param>
        /// <param name="tweenEffect"></param>
        /// <returns></returns>
        public static BlokSpawnSystem CreateInstance(List<IndividualBlokPoolingData> possibleBloksToSpawn,
            Vector2Int minPosition, Vector2Int maxPosition,
            float minTimeBetweenSpawn, float maxTimeBetweenSpawn, LayerMask wallLayers, LayerMask enemyLayers,
            TweenEffect tweenEffect)
        {
            var data = ScriptableObject.CreateInstance<BlokSpawnSystem>();
            data.Init(possibleBloksToSpawn, minPosition, maxPosition, minTimeBetweenSpawn, maxTimeBetweenSpawn, wallLayers, enemyLayers, tweenEffect);
            return data;
        }

        /// <summary>
        /// Initializer that is called when creating this object.
        /// </summary>
        /// <param name="possibleBloksToSpawn"></param>
        /// <param name="minPosition"></param>
        /// <param name="maxPosition"></param>
        /// <param name="minTimeBetweenSpawn"></param>
        /// <param name="maxTimeBetweenSpawn"></param>
        /// <param name="wallLayers"></param>
        /// <param name="enemyLayers"></param>
        /// <param name="tweenEffect"></param>
        private void Init(List<IndividualBlokPoolingData> possibleBloksToSpawn, Vector2Int minPosition, Vector2Int maxPosition, float minTimeBetweenSpawn, float maxTimeBetweenSpawn, LayerMask wallLayers, LayerMask enemyLayers, TweenEffect tweenEffect)
        {
            this.possibleBloksToSpawn = possibleBloksToSpawn;
            this.minPosition = minPosition;
            this.maxPosition = maxPosition;
            this.minTimeBetweenSpawn = minTimeBetweenSpawn;
            this.maxTimeBetweenSpawn = maxTimeBetweenSpawn;
            this.wallLayers = wallLayers;
            this.enemyLayers = enemyLayers;
            this.tweenEffect = tweenEffect;
        }


        public IEnumerator SpawnBlok(GameObject blokToSpawn, Transform parent)
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
            blokToSpawn.transform.parent = parent;
            blokToSpawn.SetActive(true);
            
            if (enemyCheck && enemyCheck.transform.CompareTag("Enemy") && blokToSpawn.TryGetComponent(out MoveableBlokControl blokControl))
                blokControl.HitEnemy(enemyCheck.transform);
            
            //Notify any listeners on blok that it is respawning
            blokToSpawn.GetComponent<BlokHealth>().InvokeRespawnBlok();
            
            if (tweenEffect) tweenEffect.ExecuteEffect(blokToSpawn.transform);
        }

        public IndividualBlokPoolingData GetRandomBlokTypeToSpawn()
        {
            return possibleBloksToSpawn[Random.Range(0, possibleBloksToSpawn.Count)];
        }
    }
}