using UnityEngine;
using Sirenix.OdinInspector;

namespace Bonkers.EnemySpawnManagement
{
    public class EnemySpawner : MonoBehaviour
    {
        [InlineEditor]
        [SerializeField] private EnemySpawnSystem enemySpawnSystem;
        [SerializeField] private TurbHolder turbHolder;
        [SerializeField] private GrubberHolder grubberHolder;
        [SerializeField] private SlimoHolder slimoHolder;
        [SerializeField] private ToxicSlimoHolder toxicSlimoHolder;
        [SerializeField] private GhostlyGrubberHolder ghostlyGrubberHolder;

        private void Start()
        {
            enemySpawnSystem.InitializeSystem();
            StartCoroutine(enemySpawnSystem.SpawnLifeforms(enemySpawnSystem.TurbPrefab,
                enemySpawnSystem.MinTurbSpawnWait, enemySpawnSystem.MaxTurbSpawnWait, enemySpawnSystem.MaxTurbs,
                turbHolder, enemySpawnSystem.SpawnTurbs));
            StartCoroutine(enemySpawnSystem.SpawnLifeforms(enemySpawnSystem.GrubberPrefab, 
                enemySpawnSystem.MinGrubberSpawnWait, enemySpawnSystem.MaxGrubberSpawnWait, enemySpawnSystem.MaxGrubbers,
                grubberHolder, enemySpawnSystem.SpawnGrubbers));
            StartCoroutine(enemySpawnSystem.SpawnLifeforms(enemySpawnSystem.TarSlimoPrefab, 
                enemySpawnSystem.MinTarSlimoSpawnWait, enemySpawnSystem.MaxTarSlimoSpawnWait, enemySpawnSystem.MaxTarSlimos,
                slimoHolder, enemySpawnSystem.SpawnTarSlimos));
            StartCoroutine(enemySpawnSystem.SpawnLifeforms(enemySpawnSystem.ToxicSlimoPrefab, 
                enemySpawnSystem.MinToxicSlimoSpawnWait, enemySpawnSystem.MaxToxicSlimoSpawnWait, enemySpawnSystem.MaxToxicSlimos,
                toxicSlimoHolder, enemySpawnSystem.SpawnToxicSlimos));
            StartCoroutine(enemySpawnSystem.SpawnLifeforms(enemySpawnSystem.GhostlyGrubberPrefab, 
                enemySpawnSystem.MinGhostlyGrubberSpawnWait, enemySpawnSystem.MaxGhostlyGrubberSpawnWait, enemySpawnSystem.MaxGhostlyGrubbers,
                ghostlyGrubberHolder, enemySpawnSystem.SpawnGhostlyGrubbers));
            
        }
        
        
    }
}