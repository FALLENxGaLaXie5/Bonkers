#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Bonkers.BlokControl;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Bonkers.ContentGeneration
{
    public abstract class ContentGenerationAgent : Agent
    {
        [SerializeField] protected ContentGeneratorData contentGeneratorData;
        [SerializeField] protected GeneratedDataSystem generatedDataSystem;
        public ContentLevelMapping contentLevelMapping;
        public int currentMapSize = 0;

        #region Events
        
        public event Action OnBeginEpisode;
        public event Action OnEndEpisode;
        
        #endregion
        
        protected BlokMapping lastBlokMappingChain1;
        protected BlokMapping lastBlokMappingChain2;
        protected BlokMapping lastBlokMappingChain3;
        protected BlokMapping lastBlokMappingChain4;
        
        protected Dictionary<Vector2Int, bool> lastBlokMappingAdjacentPositionsAvailableChain1 =
            new Dictionary<Vector2Int, bool>();
        protected Dictionary<Vector2Int, bool> lastBlokMappingAdjacentPositionsAvailableChain2 =
            new Dictionary<Vector2Int, bool>();
        protected Dictionary<Vector2Int, bool> lastBlokMappingAdjacentPositionsAvailableChain3 =
            new Dictionary<Vector2Int, bool>();
        protected Dictionary<Vector2Int, bool> lastBlokMappingAdjacentPositionsAvailableChain4 =
            new Dictionary<Vector2Int, bool>();


        protected Vector3Int heuristicInputs = Vector3Int.zero;
        protected Transform contentObjectsParent;
        protected bool setEpisodeToEnd = false;


        #region Monobehavior Lifecycle Functions

        protected override void OnEnable()
        {
            base.OnEnable();
            OnEndEpisode += AttemptRecordNewContentData;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            OnEndEpisode -= AttemptRecordNewContentData;
        }

        #endregion
        
        #region Agent Functions
        
        public override void OnEpisodeBegin()
        {
            lastBlokMappingAdjacentPositionsAvailableChain1.Clear();
            lastBlokMappingAdjacentPositionsAvailableChain2.Clear();
            lastBlokMappingAdjacentPositionsAvailableChain3.Clear();
            lastBlokMappingAdjacentPositionsAvailableChain4.Clear();

            ResetEpisodeEndingFlag();

            OnBeginEpisode?.Invoke();
            contentLevelMapping = new ContentLevelMapping();
            currentMapSize = contentGeneratorData.RandomLevelSize;

            //If we are spawning the actual objects for a visual representation, assign the content objects parent
            if (contentGeneratorData.SpawnContentGeneratorPrefabs.Value)
            {
                contentObjectsParent = Instantiate(contentGeneratorData.ContentParentPrefab, Vector3.zero, Quaternion.identity, transform).transform;
            }
            
            //Generate 4 initial bloks to be the start of the chains
            int randomType1 = Random.Range(0, 6);
            BlokMapping blokMapping1 = new BlokMapping(new Vector2Int(Random.Range(1, currentMapSize - 1), 1), contentGeneratorData.BlokTypesLookUp[randomType1]);
            contentLevelMapping.masterPositionMappingsStructure[randomType1].Add(blokMapping1);
            lastBlokMappingChain1 = blokMapping1;
            SpawnObject(blokMapping1.BlokType, blokMapping1.Value);
            CalculateNewAdjacentPositionsAvailability(blokMapping1, 0);

            int randomType2 = Random.Range(0, 6);
            BlokMapping blokMapping2 = new BlokMapping(new Vector2Int(currentMapSize - 2, Random.Range(1, currentMapSize - 1)), contentGeneratorData.BlokTypesLookUp[randomType2]);
            contentLevelMapping.masterPositionMappingsStructure[randomType2].Add(blokMapping2);
            lastBlokMappingChain2 = blokMapping2;
            SpawnObject(blokMapping2.BlokType, blokMapping2.Value);
            CalculateNewAdjacentPositionsAvailability(blokMapping2, 1);

            int randomType3 = Random.Range(0, 6);
            BlokMapping blokMapping3 = new BlokMapping(new Vector2Int(Random.Range(1, currentMapSize - 1), currentMapSize - 2), contentGeneratorData.BlokTypesLookUp[randomType3]);
            contentLevelMapping.masterPositionMappingsStructure[randomType3].Add(blokMapping3);
            lastBlokMappingChain3 = blokMapping3;
            SpawnObject(blokMapping3.BlokType, blokMapping3.Value);
            CalculateNewAdjacentPositionsAvailability(blokMapping3, 2);

            int randomType4 = Random.Range(0, 6);
            BlokMapping blokMapping4 = new BlokMapping(new Vector2Int(1, Random.Range(1, currentMapSize - 1)), contentGeneratorData.BlokTypesLookUp[randomType4]);
            contentLevelMapping.masterPositionMappingsStructure[randomType4].Add(blokMapping4);
            lastBlokMappingChain4 = blokMapping4;
            SpawnObject(blokMapping4.BlokType, blokMapping4.Value);
            CalculateNewAdjacentPositionsAvailability(blokMapping4, 3);
        }
        
        public override void CollectObservations(VectorSensor sensor)
        {
            //1 observation for size of each list of instantiated positions
            //Look at ContentLevelMapping to see how many are currently being added (as of Sept 14, it's 6)
            foreach (var blockMappingsList in contentLevelMapping.masterPositionMappingsStructure)
            {
                sensor.AddObservation(blockMappingsList.Value.Count);
            }

            //Observe available adjacent positions relative to last position (4 or 8 right now)
            foreach (var availableAdjacency in lastBlokMappingAdjacentPositionsAvailableChain1)
            {
                if (availableAdjacency.Value) sensor.AddObservation(1);
                else sensor.AddObservation(0);
            }
            //Observe available adjacent positions relative to last position (4 or 8 right now)
            foreach (var availableAdjacency in lastBlokMappingAdjacentPositionsAvailableChain2)
            {
                if (availableAdjacency.Value) sensor.AddObservation(1);
                else sensor.AddObservation(0);
            }
            //Observe available adjacent positions relative to last position (4 or 8 right now)
            foreach (var availableAdjacency in lastBlokMappingAdjacentPositionsAvailableChain3)
            {
                if (availableAdjacency.Value) sensor.AddObservation(1);
                else sensor.AddObservation(0);
            }
            //Observe available adjacent positions relative to last position (4 or 8 right now)
            foreach (var availableAdjacency in lastBlokMappingAdjacentPositionsAvailableChain4)
            {
                if (availableAdjacency.Value) sensor.AddObservation(1);
                else sensor.AddObservation(0);
            }

            //Add where the last block was instantiated (2 observation space since a vector2)
            sensor.AddObservation(lastBlokMappingChain1.Value);
            //Add where the last block was instantiated (2 observation space since a vector2)
            sensor.AddObservation(lastBlokMappingChain2.Value);
            //Add where the last block was instantiated (2 observation space since a vector2)
            sensor.AddObservation(lastBlokMappingChain3.Value);
            //Add where the last block was instantiated (2 observation space since a vector2)
            sensor.AddObservation(lastBlokMappingChain4.Value);
            
            //Observe what the current map size is set to
            sensor.AddObservation(currentMapSize);
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
            Vector3Int newHeuristicInputs = new Vector3Int(heuristicInputs.x, heuristicInputs.y, heuristicInputs.z);
            discreteActions[0] = newHeuristicInputs.x;
            discreteActions[1] = newHeuristicInputs.y;
            discreteActions[2] = newHeuristicInputs.z;
        }

        #endregion

        #region Reward Functions

        /// <summary>
        /// Rewards/punishes/ends level based on if position is free or not
        /// </summary>
        /// <param name="blockMappingPosition"></param>
        protected abstract void EvaluateIfUnusedPosition(Vector2Int blockMappingPosition, int blockChain);

        /// <summary>
        /// Rewards/punishes based on if it is inside boundaries of current level size being generated
        /// </summary>
        /// <param name="blockMappingPosition"></param>
        protected virtual void EvaluateIfInsideBoundaries(Vector2Int blockMappingPosition)
        {
            if (blockMappingPosition.x < 1 || blockMappingPosition.x > currentMapSize - 2 || blockMappingPosition.y < 1 || blockMappingPosition.y > currentMapSize - 2)
            {
                AddReward(-1f);
                InitiateEpisodeEnding();
                return;
            }
            AddReward(1f/ContentGenerationAgentUtility.CalculateMaxNumberInnerBloks(currentMapSize));
        }
        #endregion

        #region Class Functions

        /// <summary>
        /// Repopulates the available adjacent positions structure based on which are available
        /// </summary>
        /// <param name="blokMapping"></param>
        protected abstract void CalculateNewAdjacentPositionsAvailability(BlokMapping blokMapping, int blockChain);

        /// <summary>
        /// Triggers all episode end logic, along with the "OnEpisodeEnd" event
        /// </summary>
        protected virtual void TriggerEpisodeEnd()
        {
            //EpisodeEndRewards();
            EpisodeEndLogging();
            OnEndEpisode?.Invoke();
            if(contentObjectsParent) Destroy(contentObjectsParent.gameObject);
            EndEpisode();
        }

        /// <summary>
        /// All logging statements that will be printed (if debugging is on) at the end of each content
        /// generating episode.
        /// </summary>
        protected virtual void EpisodeEndLogging()
        {
            DebugStatement("Episode ended!");
            DebugStatement("Boundary Bloks Total: " + contentLevelMapping.immovableBlokMappings.Count);
            DebugStatement("Non Boundary Bloks Total: " + contentLevelMapping.GetTotalNumberNonImmovableBloks());
        }

        /// <summary>
        /// Custom debug log to use only if debugging is turned on in the generation data scriptable object
        /// </summary>
        /// <param name="text"></param>
        protected virtual void DebugStatement(string text)
        {
            if (!contentGeneratorData.DebugStatementsOn.Value) return;
            Debug.Log(text);
        }
        
        /// <summary>
        /// Used for when we want to physically spawn the bloks as a visual
        /// </summary>
        /// <param name="blokType"></param>
        /// <param name="blokMappingPosition"></param>
        protected virtual void SpawnObject(IndividualBlokPoolingData blokType, Vector2Int blokMappingPosition)
        {
            if (!contentGeneratorData.SpawnContentGeneratorPrefabs.Value) return;
            
            GameObject newBlok = Instantiate(blokType.Prefab, Vector3.zero, Quaternion.identity, transform);
            newBlok.transform.localPosition = new Vector3(blokMappingPosition.x, blokMappingPosition.y, 0f);
            newBlok.transform.parent = contentObjectsParent;
        }

        /// <summary>
        /// This is used to flag for the episode to end at the end of the action phase of cylce
        /// </summary>
        protected void InitiateEpisodeEnding() => setEpisodeToEnd = true;

        /// <summary>
        /// Resets episode ending
        /// </summary>
        protected void ResetEpisodeEndingFlag() => setEpisodeToEnd = false;
        
        /// <summary>
        /// Sets the global heuristic inputs.
        /// </summary>
        /// <param name="heuristicInputs"></param>
        public void SetHeuristicInputs(Vector3Int heuristicInputs) => this.heuristicInputs = heuristicInputs;

        /// <summary>
        /// The generated data system will record the new content level mapping when an episode ends.
        /// </summary>
        protected virtual void AttemptRecordNewContentData()
        {
            if (!generatedDataSystem)
            {
                DebugStatement("There is no generated data system set!");
                return;
            }
            
            generatedDataSystem.AttemptRecordNewContentData(contentLevelMapping, currentMapSize);
        }
        
        #endregion
    }
}
#endif