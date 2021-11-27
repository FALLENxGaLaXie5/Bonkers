using UnityEngine;
using UnityEngine.InputSystem;

namespace Bonkers.Control
{
    [System.Serializable]
    public class PlayerConfiguration
    {
        PlayerInput playerInput { get; set; }
        int playerIndex { get; set; }
        bool isReady { get; set; }
        Material playerColor { get; set; }

        public PlayerInput PlayerInput => playerInput;
        public int PlayerIndex => playerIndex;
        public bool IsReady => isReady;
        public void SetReady(bool isReady) => this.isReady = isReady;
        public Material PlayerColor => playerColor;
        public void SetPlayerColor(Material playerColor) => this.playerColor = playerColor;

        public PlayerConfiguration(PlayerInput playerInput)
        {
            playerIndex = playerInput.playerIndex;
            this.playerInput = playerInput;
        }
    }
}