#if UNITY_EDITOR
using System;
using UnityEngine;

namespace Bonkers.ContentGeneration
{
    public static class ArrayUtilities
    {
        /// <summary>
        /// Initialize a new 2D array with a default value with x size and y size.
        /// </summary>
        /// <param name="xSize"></param>
        /// <param name="ySize"></param>
        /// <param name="initialValue"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T[,] GetNew2DArray<T>(int xSize, int ySize, T initialValue)
        {
            T[,] nums = new T[xSize, ySize];
            for (int i = 0; i < xSize * ySize; i++) nums[i % xSize, i / xSize] = initialValue;
            return nums;
        }
        
        public static BlokMapping[,] GetNew2DArrayBlokMappings<T>(int xSize, int ySize, BlokMapping initialValue)
        {
            BlokMapping[,] nums = new BlokMapping[xSize, ySize];
            for (int i = 0; i < xSize * ySize; i++) nums[i % xSize, i / xSize] = 
                new BlokMapping(new Vector2Int(i % xSize, i / xSize), initialValue.BlokType);
            return nums;
        }

        /// <summary>
        /// Used to subscribe all the blok mappings' 'change to spawn blok' action to a particular function
        /// </summary>
        /// <param name="blokMappingsArray"></param>
        /// <param name="function"></param>
        public static void SubscribeBlokMappingsSpawnerChange(BlokMapping[,] blokMappingsArray, Action<BlokMapping> function)
        {
            for (int c = 0; c < blokMappingsArray.GetLength(0); c++)
            {
                for (int r = 0; r < blokMappingsArray.GetLength(1); r++)
                {
                    blokMappingsArray[c, r].OnChangeToSpawnerBlok += function;
                }
            }
        }
        
        /// <summary>
        /// Used to unsubscribe all the blok mappings' 'change to spawn blok' action to a particular function
        /// </summary>
        /// <param name="blokMappingsArray"></param>
        /// <param name="function"></param>
        public static void UnsubscribeBlokMappingsSpawnerChange(BlokMapping[,] blokMappingsArray, Action<BlokMapping> function)
        {
            for (int c = 0; c < blokMappingsArray.GetLength(0); c++)
            {
                for (int r = 0; r < blokMappingsArray.GetLength(1); r++)
                {
                    blokMappingsArray[c, r].OnChangeToSpawnerBlok -= function;
                }
            }
        }
    }
}
#endif