#if UNITY_EDITOR

using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.MLAgents.Policies;

namespace Bonkers.ContentGeneration.UI
{
    public class BlokGenerationHeuristicUI : MonoBehaviour
    {
        [SerializeField] private TMP_InputField blokTypeInputField;
        //[SerializeField] private TMP_InputField blokXPositionInputField;
        //[SerializeField] private TMP_InputField blokYPositionInputField;
        [SerializeField] private TMP_Dropdown spawnDirectionDropdownField;
        [SerializeField] private TMP_Dropdown blokChainDropdownField;
        [SerializeField] private Button submitButton;
        [SerializeField] private ContentGenerationDecisionRequester decisionRequester;
        [SerializeField] private BehaviorParameters behaviorParameters;

        private void Start()
        {
            if (!behaviorParameters.IsInHeuristicMode())
            {
                gameObject.SetActive(false);
            }
        }

        private void OnEnable() => submitButton.onClick.AddListener(AttemptInvokeDecisionRequester);

        private void OnDisable() => submitButton.onClick.RemoveAllListeners();

        void AttemptInvokeDecisionRequester()
        {
            bool invokeDecision = true;
            int blokType;

            if (!int.TryParse(blokTypeInputField.text, out blokType))
            {
                blokTypeInputField.text = "Please input an int value.";
                invokeDecision = false;
            }
            
            if (!invokeDecision) return;
            decisionRequester.RequestDecisionHeuristicMode(new Vector3Int(blokType, spawnDirectionDropdownField.value, blokChainDropdownField.value));
        }
    }
}
#endif