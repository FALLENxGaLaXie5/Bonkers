using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bonkers.SceneManagement
{
    public class MainMenu : MonoBehaviour
    {
        Portal portal;
        [SerializeField] int singlePlayerSceneIndex = 1;
        [SerializeField] int twoPlayerSceneIndex = 2;

        void Start()
        {
            portal = FindObjectOfType<Portal>();
        }

        public void PlayGameSinglePlayer()
        {
            portal.StartTransition(singlePlayerSceneIndex);
        }

        public void PlayerGameTwoPlayer()
        {
            portal.StartTransition(twoPlayerSceneIndex);
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
