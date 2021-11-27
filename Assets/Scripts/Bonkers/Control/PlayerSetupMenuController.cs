using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Bonkers.Control
{
    public class PlayerSetupMenuController : MonoBehaviour
    {
        [SerializeField] PlayerConfigurationSystem playerConfigurationSystem;
        [SerializeField] TextMeshProUGUI titleText;
        [SerializeField] GameObject readyPanel;
        [SerializeField] GameObject menuPanel;
        [SerializeField] Button readyButton;
        [SerializeField] Button startLevelButton;

        int playerIndex;
        float ignoreInputTime = 1.5f;
        bool inputEnabled;
        bool localPlayer1 = false;

        void Start()
        {
            if(playerConfigurationSystem.playerConfigs.Count < 2) localPlayer1 = true;
        }

        // Update is called once per frame
        void Update()
        {
            if (Time.time > ignoreInputTime)
            {
                inputEnabled = true;
            }
        }

        public void SetPlayerIndex(int playerIndex)
        {
            this.playerIndex = playerIndex;
            titleText.SetText("Player " + (playerIndex + 1).ToString());
            ignoreInputTime = Time.time + ignoreInputTime;
        }

        public void SetColor(Material color)
        {
            if (!inputEnabled) return;
            playerConfigurationSystem.SetPlayerColor(playerIndex, color);
            readyPanel.SetActive(true);
            readyButton.Select();
            menuPanel.SetActive(false);
        }

        public void ReadyPlayer()
        {
            if (!inputEnabled) return;
            playerConfigurationSystem.ReadyPlayer(playerIndex);
            readyButton.gameObject.SetActive(false);
            if(localPlayer1)
            {
                startLevelButton.gameObject.SetActive(true);
                startLevelButton.Select();
            }            
        }

        public void AttemptStartLevel()
        {
            if (!inputEnabled) return;
            playerConfigurationSystem.StartLevel();
        }
    }
}
