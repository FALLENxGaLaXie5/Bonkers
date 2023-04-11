using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

namespace Bonkers.Core
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] string menuSceneName = "MainMenu";
        public GameObject pauseMenuUI;
        public static bool gameIsPaused = false;

        InputMaster controls;

        void Awake()
        {
            controls = new InputMaster();    
        }

        void OnEnable()
        {
            controls.Player.Pause.performed += context => TriggerPauseMenu();
            controls.Enable();
        }

        void OnDisable()
        {
            controls.Player.Pause.performed -= context => TriggerPauseMenu();
            controls.Disable();
        }

        void TriggerPauseMenu()
        {
            if (gameIsPaused) Resume();
            else Pause();
        }

        public void Resume()
        {
            pauseMenuUI.SetActive(false);
            Time.timeScale = 1f;
            gameIsPaused = false;
        }

        public void Pause()
        {
            pauseMenuUI.SetActive(true);
            Time.timeScale = 0f;
            gameIsPaused = true;
        }

        public void LoadMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(menuSceneName);
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}