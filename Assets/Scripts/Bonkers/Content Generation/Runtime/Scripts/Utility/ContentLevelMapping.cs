
#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Bonkers.BlokControl;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Experimental.AI;
using UnityEngine.Serialization;

namespace Bonkers.ContentGeneration
{
    [Serializable]
    public class ContentLevelMapping
    {
        public Dictionary<int, List<BlokMapping>> masterPositionMappingsStructure;
        
        //These lists need to match what's in the Content Generator Data
        [ReadOnly] public List<BlokMapping> immovableBlokMappings;
        [ReadOnly] public List<BlokMapping> basicBlokPositionMappings;
        [ReadOnly] public List<BlokMapping> bombBlokPositionMappings;
        [ReadOnly] public List<BlokMapping> glassBlokPositionMappings;
        [ReadOnly] public List<BlokMapping> iceBlokPositionMappings;
        [ReadOnly] public List<BlokMapping> woodBlokPositionMappings;


        public ContentLevelMapping()
        {
            immovableBlokMappings = new List<BlokMapping>();
            basicBlokPositionMappings = new List<BlokMapping>();
            bombBlokPositionMappings = new List<BlokMapping>();
            glassBlokPositionMappings = new List<BlokMapping>();
            iceBlokPositionMappings = new List<BlokMapping>();
            woodBlokPositionMappings = new List<BlokMapping>();

            masterPositionMappingsStructure = new Dictionary<int, List<BlokMapping>>();
            
            masterPositionMappingsStructure.Add(0, immovableBlokMappings);
            masterPositionMappingsStructure.Add(1, basicBlokPositionMappings);
            masterPositionMappingsStructure.Add(2, bombBlokPositionMappings);
            masterPositionMappingsStructure.Add(3, glassBlokPositionMappings);
            masterPositionMappingsStructure.Add(4, iceBlokPositionMappings);
            masterPositionMappingsStructure.Add(5, woodBlokPositionMappings);
        }

        public ContentLevelMapping(List<BlokMapping> immovableBlokMappings, List<BlokMapping> basicBlokPositionMappings, 
            List<BlokMapping> bombBlokPositionMappings, List<BlokMapping> glassBlokPositionMappings,
            List<BlokMapping> iceBlokPositionMappings, List<BlokMapping> woodBlokPositionMappings)
        {
            this.immovableBlokMappings = new List<BlokMapping>();
            this.basicBlokPositionMappings = new List<BlokMapping>();
            this.bombBlokPositionMappings = new List<BlokMapping>();
            this.glassBlokPositionMappings = new List<BlokMapping>();
            this.iceBlokPositionMappings = new List<BlokMapping>();
            this.woodBlokPositionMappings = new List<BlokMapping>();
            
            this.immovableBlokMappings.AddRange(immovableBlokMappings);
            this.basicBlokPositionMappings.AddRange(basicBlokPositionMappings);
            this.bombBlokPositionMappings.AddRange(bombBlokPositionMappings);
            this.glassBlokPositionMappings.AddRange(glassBlokPositionMappings);
            this.iceBlokPositionMappings.AddRange(iceBlokPositionMappings);
            this.woodBlokPositionMappings.AddRange(woodBlokPositionMappings);

            masterPositionMappingsStructure = new Dictionary<int, List<BlokMapping>>();
            
            masterPositionMappingsStructure.Add(0, immovableBlokMappings);
            masterPositionMappingsStructure.Add(1, basicBlokPositionMappings);
            masterPositionMappingsStructure.Add(2, bombBlokPositionMappings);
            masterPositionMappingsStructure.Add(3, glassBlokPositionMappings);
            masterPositionMappingsStructure.Add(4, iceBlokPositionMappings);
            masterPositionMappingsStructure.Add(5, woodBlokPositionMappings);
        }
        
        /// <summary>
        /// Add a new blok to the mapping structure
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="blokData"></param>
        public void AddBlok(int x, int y, IndividualBlokPoolingData blokData)
        {
            BlokMapping blokMapping = new BlokMapping(new Vector2Int(x, y), blokData);
            masterPositionMappingsStructure[blokData.Key].Add(blokMapping);
        }

        /// <summary>
        /// This will check all the position mappings for if the new position is already taken
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool CheckIfPositionIsAlreadyTaken(Vector2Int position)
        {
            foreach (var currentPositionMapping in masterPositionMappingsStructure)
            {
                foreach (var blokMapping in currentPositionMapping.Value)
                {
                    if (blokMapping.Value == position)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Calculate total number of bloks in the mapping
        /// </summary>
        /// <returns></returns>
        public int GetTotalNumberBloks()
        {
            int i = 0;
            foreach (var positionMappingsList in masterPositionMappingsStructure)
            {
                i += positionMappingsList.Value.Count;
            }
            return i;
        }

        /// <summary>
        /// Grabs current total number of all bloks, not including the boundary bloks
        /// </summary>
        /// <returns></returns>
        public int GetTotalNumberNonImmovableBloks() => GetTotalNumberBloks() - immovableBlokMappings.Count;

        /// <summary>
        /// Calculates a punishment for having large differences in numbers of bloks
        /// Do not include the boundary bloks in this punishment
        /// </summary>
        /// <returns></returns>
        public float CalculateBlokStructureEqualityPunishment()
        {
            int maxDifference = 0;
            for (int i = 2; i < masterPositionMappingsStructure.Count; i++)
            {
                int previousPositionListCount = masterPositionMappingsStructure[i - 1].Count;
                int currentMappingListCount = masterPositionMappingsStructure[i].Count;
                
                //Size difference between the current list of blok grids and the previous list of blok grids
                int currentDifference = currentMappingListCount - previousPositionListCount;
                //If this difference in counts is greater than the current maximum difference in mapping counts, then make it the max
                if (currentDifference > maxDifference) maxDifference = currentDifference;
            }

            return -maxDifference;
        }
    }
}
#endif