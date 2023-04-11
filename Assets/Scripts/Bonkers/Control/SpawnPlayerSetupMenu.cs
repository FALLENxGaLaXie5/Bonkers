using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace Bonkers.Control
{
    public class SpawnPlayerSetupMenu : MonoBehaviour
    {
        public GameObject playerSetupMenuPrefab;
        public PlayerInput playerInput;

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
            GameObject rootMenu = GameObject.Find("MainLayout");
            if (rootMenu)
            {
                GameObject menu = Instantiate(playerSetupMenuPrefab, rootMenu.transform);
                playerInput.uiInputModule = menu.GetComponentInChildren<InputSystemUIInputModule>();
                menu.GetComponent<PlayerSetupMenuController>().SetPlayerIndex(playerInput.playerIndex);
            }
        }
    }

}