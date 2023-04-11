#if UNITY_EDITOR
using System.Collections.Generic;
using Bonkers.BlokControl;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace Bonkers.ContentGeneration
{
    public class ContentGenerationAgentFourDirectional : ContentGenerationAgent
    {
        protected List<Vector2Int> adjacentDirections = new List<Vector2Int>
        {
            Vector2Int.up, Vector2Int.left, Vector2Int.right, Vector2Int.down
        };

        public override void OnEpisodeBegin()
        {
            base.OnEpisodeBegin();
            //Start out with all directions being available, unless they were already added by initial blocks
            foreach (var direction in adjacentDirections)
            {
                if(!lastBlokMappingAdjacentPositionsAvailableChain1.ContainsKey(direction)) 
                    lastBlokMappingAdjacentPositionsAvailableChain1.Add(direction, true);
                if(!lastBlokMappingAdjacentPositionsAvailableChain2.ContainsKey(direction))
                    lastBlokMappingAdjacentPositionsAvailableChain2.Add(direction, true);
                if(!lastBlokMappingAdjacentPositionsAvailableChain3.ContainsKey(direction))
                    lastBlokMappingAdjacentPositionsAvailableChain3.Add(direction, true);
                if(!lastBlokMappingAdjacentPositionsAvailableChain4.ContainsKey(direction))
                    lastBlokMappingAdjacentPositionsAvailableChain4.Add(direction, true);
            }
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            //Type of blok to spawn
            int blokTypeKey = actions.DiscreteActions[0];
            //Int that maps to the direction the new blok will spawn in
            int directionToSpawnIn = actions.DiscreteActions[1];
            //Int for the block chain to spawn block in
            int blockChain = actions.DiscreteActions[2];

            Vector2Int blockMappingPosition = Vector2Int.zero;
            
            switch (blockChain)
            {
                case 0:
                    blockMappingPosition = ContentGenerationAgentUtility.CalculateBlokMappingPositionFourDirectional(directionToSpawnIn, lastBlokMappingChain1, adjacentDirections);
                    break;
                case 1:
                    blockMappingPosition = ContentGenerationAgentUtility.CalculateBlokMappingPositionFourDirectional(directionToSpawnIn, lastBlokMappingChain2, adjacentDirections);
                    break;
                case 2:
                    blockMappingPosition = ContentGenerationAgentUtility.CalculateBlokMappingPositionFourDirectional(directionToSpawnIn, lastBlokMappingChain3, adjacentDirections);
                    break;
                case 3:
                    blockMappingPosition = ContentGenerationAgentUtility.CalculateBlokMappingPositionFourDirectional(directionToSpawnIn, lastBlokMappingChain4, adjacentDirections);
                    break;
            }
            


            //Create new blok mapping from the position and blok type key we receive from the actions
            IndividualBlokPoolingData blokType = contentGeneratorData.BlokTypesLookUp[blokTypeKey];
            BlokMapping blokMapping = new BlokMapping(blockMappingPosition, blokType);

            //Spawns a new blok prefab representing the new blok mapping (only if spawn prefabs is turned on in the generation data object)
            SpawnObject(blokType, blockMappingPosition);
            
            //Rewards before blok mapping gets added to the mappings structure
            EvaluateIfUnusedPosition(blockMappingPosition, blockChain);
            EvaluateIfInsideBoundaries(blockMappingPosition);
            
            //Add to the blok mappings structure in our content level mapping object for this episode
            //(only if this mapping didn't trigger episode to end and it is not set to skip this space)
            if (!setEpisodeToEnd) contentLevelMapping.masterPositionMappingsStructure[blokTypeKey].Add(blokMapping);

            //Set the last blok mapping to this one we just generated
            switch (blockChain)
            {
                case 0:
                    lastBlokMappingChain1 = blokMapping;
                    //Clear adjacent position availabilities, populate with new ones based on current new blok adjacent positions
                    lastBlokMappingAdjacentPositionsAvailableChain1.Clear();
                    break;
                case 1:
                    lastBlokMappingChain2 = blokMapping;
                    //Clear adjacent position availabilities, populate with new ones based on current new blok adjacent positions
                    lastBlokMappingAdjacentPositionsAvailableChain2.Clear();
                    break;
                case 2:
                    lastBlokMappingChain3 = blokMapping;
                    //Clear adjacent position availabilities, populate with new ones based on current new blok adjacent positions
                    lastBlokMappingAdjacentPositionsAvailableChain3.Clear();
                    break;
                case 3:
                    lastBlokMappingChain4 = blokMapping;
                    //Clear adjacent position availabilities, populate with new ones based on current new blok adjacent positions
                    lastBlokMappingAdjacentPositionsAvailableChain4.Clear();
                    break;
            }

            CalculateNewAdjacentPositionsAvailability(blokMapping, blockChain);

            //firstBlok = false;
            
            //Just for some initial training- end to episode and reward will be given when we reach max num of boundaries
            //or total max num of bloks
            if (contentLevelMapping.GetTotalNumberBloks() >= ContentGenerationAgentUtility.CalculateMaxNumberInnerBloks(currentMapSize))
            {
                AddReward(1f);
                InitiateEpisodeEnding();
            }

            if (setEpisodeToEnd) TriggerEpisodeEnd();
        }
        
        /// <summary>
        /// Rewards/punishes/ends level based on if position is free or not
        /// </summary>
        /// <param name="blockMappingPosition"></param>
        protected override void EvaluateIfUnusedPosition(Vector2Int blockMappingPosition, int blockChain)
        {
            //If this is the first blok and it's inside, nothing
            //if (firstBlok) return;

            int i = 0;
            switch (blockChain)
            {
                case 0:
                    //Reward if position is unused, punish and end episode if it's taken
                    foreach (var adjacencyAvailability in lastBlokMappingAdjacentPositionsAvailableChain1)
                    {
                        if (blockMappingPosition != lastBlokMappingChain1.Value && blockMappingPosition == adjacentDirections[i] + lastBlokMappingChain1.Value && adjacencyAvailability.Value)
                        {
                            AddReward(1f/ContentGenerationAgentUtility.CalculateMaxNumberInnerBloks(currentMapSize));
                            return;
                        }
                        i++;
                    }
                    break;
                case 1:
                    //Reward if position is unused, punish and end episode if it's taken
                    foreach (var adjacencyAvailability in lastBlokMappingAdjacentPositionsAvailableChain2)
                    {
                        if (blockMappingPosition != lastBlokMappingChain2.Value && blockMappingPosition == adjacentDirections[i] + lastBlokMappingChain2.Value && adjacencyAvailability.Value)
                        {
                            AddReward(1f/ContentGenerationAgentUtility.CalculateMaxNumberInnerBloks(currentMapSize));
                            return;
                        }
                        i++;
                    }
                    break;
                case 2:
                    //Reward if position is unused, punish and end episode if it's taken
                    foreach (var adjacencyAvailability in lastBlokMappingAdjacentPositionsAvailableChain3)
                    {
                        if (blockMappingPosition != lastBlokMappingChain3.Value && blockMappingPosition == adjacentDirections[i] + lastBlokMappingChain3.Value && adjacencyAvailability.Value)
                        {
                            AddReward(1f/ContentGenerationAgentUtility.CalculateMaxNumberInnerBloks(currentMapSize));
                            return;
                        }
                        i++;
                    }
                    break;
                case 3:
                    //Reward if position is unused, punish and end episode if it's taken
                    foreach (var adjacencyAvailability in lastBlokMappingAdjacentPositionsAvailableChain4)
                    {
                        if (blockMappingPosition != lastBlokMappingChain4.Value && blockMappingPosition == adjacentDirections[i] + lastBlokMappingChain4.Value && adjacencyAvailability.Value)
                        {
                            AddReward(1f/ContentGenerationAgentUtility.CalculateMaxNumberInnerBloks(currentMapSize));
                            return;
                        }
                        i++;
                    }
                    break;
            }
            
            AddReward(-1f);
            InitiateEpisodeEnding();
        }
        
        /// <summary>
        /// Repopulates the available adjacent positions structure based on which are available
        /// </summary>
        /// <param name="blokMapping"></param>
        protected override void CalculateNewAdjacentPositionsAvailability(BlokMapping blokMapping, int blockChain)
        {
            switch (blockChain)
            {
                case 0:
                    Vector2Int upPosition = blokMapping.Value + Vector2Int.up;
                    bool upPositionTaken = !contentLevelMapping.CheckIfPositionIsAlreadyTaken(upPosition);
                    lastBlokMappingAdjacentPositionsAvailableChain1.Add(adjacentDirections[0], upPositionTaken);

                    Vector2Int leftPosition = blokMapping.Value + Vector2Int.left;
                    bool leftPositionTaken = !contentLevelMapping.CheckIfPositionIsAlreadyTaken(leftPosition);
                    lastBlokMappingAdjacentPositionsAvailableChain1.Add(adjacentDirections[1], leftPositionTaken);

                    Vector2Int rightPosition = blokMapping.Value + Vector2Int.right;
                    bool rightPositionTaken = !contentLevelMapping.CheckIfPositionIsAlreadyTaken(rightPosition);
                    lastBlokMappingAdjacentPositionsAvailableChain1.Add(adjacentDirections[2], rightPositionTaken);

                    Vector2Int downPosition = blokMapping.Value + Vector2Int.down;
                    bool downPositionTaken = !contentLevelMapping.CheckIfPositionIsAlreadyTaken(downPosition);
                    lastBlokMappingAdjacentPositionsAvailableChain1.Add(adjacentDirections[3], downPositionTaken);
                    break;
                case 1:
                    upPosition = blokMapping.Value + Vector2Int.up;
                    upPositionTaken = !contentLevelMapping.CheckIfPositionIsAlreadyTaken(upPosition);
                    lastBlokMappingAdjacentPositionsAvailableChain2.Add(adjacentDirections[0], upPositionTaken);

                    leftPosition = blokMapping.Value + Vector2Int.left;
                    leftPositionTaken = !contentLevelMapping.CheckIfPositionIsAlreadyTaken(leftPosition);
                    lastBlokMappingAdjacentPositionsAvailableChain2.Add(adjacentDirections[1], leftPositionTaken);

                    rightPosition = blokMapping.Value + Vector2Int.right;
                    rightPositionTaken = !contentLevelMapping.CheckIfPositionIsAlreadyTaken(rightPosition);
                    lastBlokMappingAdjacentPositionsAvailableChain2.Add(adjacentDirections[2], rightPositionTaken);

                    downPosition = blokMapping.Value + Vector2Int.down;
                    downPositionTaken = !contentLevelMapping.CheckIfPositionIsAlreadyTaken(downPosition);
                    lastBlokMappingAdjacentPositionsAvailableChain2.Add(adjacentDirections[3], downPositionTaken);
                    break;
                case 2:
                    upPosition = blokMapping.Value + Vector2Int.up;
                    upPositionTaken = !contentLevelMapping.CheckIfPositionIsAlreadyTaken(upPosition);
                    lastBlokMappingAdjacentPositionsAvailableChain3.Add(adjacentDirections[0], upPositionTaken);

                    leftPosition = blokMapping.Value + Vector2Int.left;
                    leftPositionTaken = !contentLevelMapping.CheckIfPositionIsAlreadyTaken(leftPosition);
                    lastBlokMappingAdjacentPositionsAvailableChain3.Add(adjacentDirections[1], leftPositionTaken);

                    rightPosition = blokMapping.Value + Vector2Int.right;
                    rightPositionTaken = !contentLevelMapping.CheckIfPositionIsAlreadyTaken(rightPosition);
                    lastBlokMappingAdjacentPositionsAvailableChain3.Add(adjacentDirections[2], rightPositionTaken);

                    downPosition = blokMapping.Value + Vector2Int.down;
                    downPositionTaken = !contentLevelMapping.CheckIfPositionIsAlreadyTaken(downPosition);
                    lastBlokMappingAdjacentPositionsAvailableChain3.Add(adjacentDirections[3], downPositionTaken);
                    break;
                case 3:
                    upPosition = blokMapping.Value + Vector2Int.up;
                    upPositionTaken = !contentLevelMapping.CheckIfPositionIsAlreadyTaken(upPosition);
                    lastBlokMappingAdjacentPositionsAvailableChain4.Add(adjacentDirections[0], upPositionTaken);

                    leftPosition = blokMapping.Value + Vector2Int.left;
                    leftPositionTaken = !contentLevelMapping.CheckIfPositionIsAlreadyTaken(leftPosition);
                    lastBlokMappingAdjacentPositionsAvailableChain4.Add(adjacentDirections[1], leftPositionTaken);

                    rightPosition = blokMapping.Value + Vector2Int.right;
                    rightPositionTaken = !contentLevelMapping.CheckIfPositionIsAlreadyTaken(rightPosition);
                    lastBlokMappingAdjacentPositionsAvailableChain4.Add(adjacentDirections[2], rightPositionTaken);

                    downPosition = blokMapping.Value + Vector2Int.down;
                    downPositionTaken = !contentLevelMapping.CheckIfPositionIsAlreadyTaken(downPosition);
                    lastBlokMappingAdjacentPositionsAvailableChain4.Add(adjacentDirections[3], downPositionTaken);
                    break;
            }
        }
    }
}
#endif