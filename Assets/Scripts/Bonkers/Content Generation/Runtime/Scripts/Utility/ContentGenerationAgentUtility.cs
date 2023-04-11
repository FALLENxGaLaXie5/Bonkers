#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

namespace Bonkers.ContentGeneration
{
    public static class ContentGenerationAgentUtility
    {
        /// <summary>
        /// Calculates the correct blok position based on direction given (4 directions)
        /// </summary>
        /// <param name="directionToSpawnIn"></param>
        /// <returns></returns>
        public static Vector2Int CalculateBlokMappingPositionFourDirectional(int directionToSpawnIn, BlokMapping lastBlokMapping, List<Vector2Int> adjacentDirections)
        {
            //Generate new position for blok based on direction given by action
            Vector2Int blockMappingPosition = Vector2Int.zero;
            switch (directionToSpawnIn)
            {
                case 0:
                    blockMappingPosition = lastBlokMapping.Value + adjacentDirections[0];
                    break;
                case 1:
                    blockMappingPosition = lastBlokMapping.Value + adjacentDirections[1];
                    break;
                case 2:
                    blockMappingPosition = lastBlokMapping.Value + adjacentDirections[2];
                    break;
                case 3:
                    blockMappingPosition = lastBlokMapping.Value + adjacentDirections[3];
                    break;
            }
            return blockMappingPosition;
        }
        
        /// <summary>
        /// Calculates the correct blok position based on direction given (4 directions)
        /// </summary>
        /// <param name="directionToSpawnIn"></param>
        /// <returns></returns>
        public static Vector2Int CalculateBlokMappingPositionEightDirectional(int directionToSpawnIn, BlokMapping lastBlokMapping, List<Vector2Int> adjacentDirections)
        {
            //Generate new position for blok based on direction given by action
            Vector2Int blockMappingPosition = Vector2Int.zero;
            switch (directionToSpawnIn)
            {
                case 0:
                    blockMappingPosition = lastBlokMapping.Value + adjacentDirections[0];
                    break;
                case 1:
                    blockMappingPosition = lastBlokMapping.Value + adjacentDirections[1];
                    break;
                case 2:
                    blockMappingPosition = lastBlokMapping.Value + adjacentDirections[2];
                    break;
                case 3:
                    blockMappingPosition = lastBlokMapping.Value + adjacentDirections[3];
                    break;
                case 4:
                    blockMappingPosition = lastBlokMapping.Value + adjacentDirections[4];
                    break;
                case 5:
                    blockMappingPosition = lastBlokMapping.Value + adjacentDirections[5];
                    break;
                case 6:
                    blockMappingPosition = lastBlokMapping.Value + adjacentDirections[6];
                    break;
                case 7:
                    blockMappingPosition = lastBlokMapping.Value + adjacentDirections[7];
                    break;
            }
            return blockMappingPosition;
        }
        
        /// <summary>
        /// Total number of inner bloks (not including the boundaries) needed
        /// </summary>
        /// <returns></returns>
        public static int CalculateMaxNumberInnerBloks(int currentMapSize)
        {
            int innerPlayAreaSize = (currentMapSize - 2);
            return (int)Mathf.Ceil(innerPlayAreaSize * innerPlayAreaSize / 2f);
        }

        /// <summary>
        /// This will calculate the exact number of boundary bloks needed around the edge of map
        /// </summary>
        /// <returns></returns>
        public static int CalculateNumberNeededBoundaryBloks(int currentMapSize) => currentMapSize * 4 - 4;
    }
}
#endif