#if UNITY_EDITOR
using Bonkers.Control;
using UnityEngine;

namespace Bonkers.ContentGeneration
{
    [RequireComponent(typeof(InitializeLevel))]
    public class PlayerSpawnLevelGeneration : MonoBehaviour
    {
        [SerializeField] private InitializeLevel initializeLevelComponent;

        /// <summary>
        /// Remove any blok mappings where the player spawn positions need to go, and reposition player spawns
        /// where they need to go.
        /// </summary>
        /// <param name="mapSize"></param>
        /// <param name="generatedContentData"></param>
        public void GenerateNewPlayerSpawnPositions(int mapSize, GeneratedContentData generatedContentData)
        {
            int positionX = 1;
            int positionY = 1;
            generatedContentData.RemoveAnyBlokMappingAt(positionX, positionY);
            initializeLevelComponent.PlayerSpawns[0].position = new Vector3(positionX, positionY, 0);

            positionX = mapSize - 2;
            positionY = 1;
            generatedContentData.RemoveAnyBlokMappingAt(positionX, positionY);
            initializeLevelComponent.PlayerSpawns[1].position = new Vector3(positionX, positionY, 0);

            positionX = 1;
            positionY = mapSize - 2;
            generatedContentData.RemoveAnyBlokMappingAt(positionX, positionY);
            initializeLevelComponent.PlayerSpawns[2].position = new Vector3(positionX, positionY, 0);

            positionX = mapSize - 2;
            positionY = mapSize - 2;
            generatedContentData.RemoveAnyBlokMappingAt(positionX, positionY);
            initializeLevelComponent.PlayerSpawns[3].position = new Vector3(positionX, positionY, 0);
        }
    }
}
#endif