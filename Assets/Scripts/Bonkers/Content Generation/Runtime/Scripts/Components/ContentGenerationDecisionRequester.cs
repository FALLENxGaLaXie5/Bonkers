#if UNITY_EDITOR
using System;
using System.Collections;
using Sirenix.OdinInspector;
using Unity.MLAgents;
using Unity.MLAgents.Policies;
using UnityEngine;

namespace Bonkers.ContentGeneration
{
    [RequireComponent(typeof(ContentGenerationAgent))]
    public class ContentGenerationDecisionRequester : MonoBehaviour
    {
        [ShowIf("@this.agent == null")][SerializeField] private ContentGenerationAgent agent;
        [ShowIf("@this.behaviorParameters == null")] [SerializeField]
        private BehaviorParameters behaviorParameters;

        [SerializeField] private float decisionInterval = 0.5f;

        /// <summary>
        /// The call for requesting a decision if operating in heuristic mode
        /// </summary>
        /// <param name="inputs"></param>
        public void RequestDecisionHeuristicMode(Vector3Int inputs)
        {
            if (!behaviorParameters.IsInHeuristicMode())
            {
                Debug.Log("Not in heuristic mode!");
                return;
            }
            agent.SetHeuristicInputs(inputs);
            agent.RequestDecision();
        }

        
        private void OnEnable()
        {
            agent.OnBeginEpisode += BeginRequestingDecisions;
            agent.OnEndEpisode += EndRequestingDecisions;
        }

        private void OnDisable()
        {
            agent.OnBeginEpisode -= BeginRequestingDecisions;
            agent.OnEndEpisode -= EndRequestingDecisions;
        }
        

        /// <summary>
        /// Used for if not in heuristic mode- automatically requesting decisions
        /// </summary>
        void BeginRequestingDecisions()
        {
            //Request decisions on an interval if not in heuristic mode
            if (!behaviorParameters.IsInHeuristicMode())
            {
                StartCoroutine(RequestDecisions());
            }
        }

        void EndRequestingDecisions()
        {
            StopAllCoroutines();
        }

        private IEnumerator RequestDecisions()
        {
            WaitForSeconds waitTime = new WaitForSeconds(decisionInterval);
            while (true)
            {
                yield return new WaitForSeconds(decisionInterval);
                agent.RequestDecision();
            }
        }
    }
}
#endif