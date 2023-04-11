using System;
using Bonkers.Events;
using Bonkers.SceneManagement;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

namespace Bonkers.Control
{
    public class InitializeLevel : MonoBehaviour
    {
        [InlineEditor]
        [SerializeField] private PlayerConfigurationSystem playerConfigurationManger;
        [SerializeField] private PlayerControlManagement playerControlManagementSystem;
        [SerializeField] private Transform[] playerSpawns;
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private VoidEvent endLevelEvent;

        #region Properties
        public Transform[] PlayerSpawns => playerSpawns;
        #endregion

        void Start()
        {
            var playerConfigs = playerConfigurationManger.GetPlayerConfigs().ToArray();
            for (int i = 0; i < playerConfigs.Length; i++)
            {
                var player = Instantiate(playerPrefab, playerSpawns[i].position, playerSpawns[i].rotation, gameObject.transform);
                PlayerControl playerControl = player.GetComponent<PlayerControl>();
                playerControlManagementSystem.AddPlayer(playerControl);
                playerControl.OnPlayerDestroy += PlayerDies;
                player.GetComponent<PlayerInputHandler>().InitializePlayer(playerConfigs[i], i+1);
            }
        }
        //TODO: refactor pretty much everything in this script
#if UNITY_EDITOR
        
        private void OnEnable()
        {
            EditorApplication.playModeStateChanged += HandlePlayModeChange;
        }
        
        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= HandlePlayModeChange;
        }
        

        private void HandlePlayModeChange(PlayModeStateChange stateChange)
        {
            
            //TODO: refactor this a bit, just here for a quick solution to player configs not resetting correctly
            if (stateChange == PlayModeStateChange.EnteredPlayMode ||
                stateChange == PlayModeStateChange.ExitingPlayMode)
            {
                Debug.LogWarning("Exiting or entering play mode!");
                playerConfigurationManger.ClearPlayerConfigs();
                playerControlManagementSystem.ClearPlayers();
            }
            
        }
#endif

        public void EndLevel()
        {
            //TODO: this needs to get refactored- NO FINDOBJECTSOFTYPE!!!
            PlayerControl[] playerControls = FindObjectsOfType<PlayerControl>();
            foreach (var playerControl in playerControls)
            {
                playerControl.OnPlayerDestroy -= PlayerDies;
                playerControlManagementSystem.RemovePlayer(playerControl);
            }
            
            playerConfigurationManger.ClearDevicesBeingUsed();
            //Clear all the player configuration objects that exist on that manager
            playerConfigurationManger.ClearPlayerConfigs();
            //Portal back to main scene\

            //TODO: refactor this
            FindObjectOfType<Portal>().StartTransition(0);
        }

        private void PlayerDies(PlayerControl playerControl)
        {
            playerControl.OnPlayerDestroy -= PlayerDies;
            playerControlManagementSystem.RemovePlayer(playerControl);
            
            if (CheckIfAllPlayersDead())
            {
                endLevelEvent.Raise();
            }
        }

        private bool CheckIfAllPlayersDead()
        {
            if (playerControlManagementSystem.NumberPlayersRemaining < 1)
            {
                return true;
            }

            return false;
        }
    }
}