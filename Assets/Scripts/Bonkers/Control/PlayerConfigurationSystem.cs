using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;
using UnityEngine.SceneManagement;

namespace Bonkers.Control
{
    [CreateAssetMenu(fileName = "New Player Configuration System", menuName = "Configuration/Create New Player Configuration System")]
    public class PlayerConfigurationSystem : ScriptableObject
    {
        #region Inpsector/Public Fields

        [SerializeField] string sceneToLoad = "";
        [SerializeField] GameObject playerConfigurationWASDPrefab;
        [SerializeField] GameObject playerConfigurationArrowsPrefab;
        [SerializeField] GameObject playerConfigurationGamepadPrefab;
        
        #endregion

        #region Private/Class Fields
        public List<PlayerConfiguration> playerConfigs = new List<PlayerConfiguration>();

        List<KeyboardSetup> keyboardDevices = new List<KeyboardSetup>();
        List<GamepadSetup> gamepadDevices = new List<GamepadSetup>();

        #endregion



        #region Unity Callbacks

        public void Initialize() => InputSystem.onEvent += OnCheckInput;
        public void Deinitialize() => InputSystem.onEvent -= OnCheckInput;
        //void OnEnable() => 

        //void OnDisable() => InputSystem.onEvent -= OnCheckInput;

        #endregion

        #region Class Functions
        
        /// <summary>
        /// Check raw input, will vet for state and delta state events.
        ///
        /// Will Check if it's keyboard or gamepad input, and route accordingly
        /// </summary>
        /// <param name="eventPtr"></param>
        /// <param name="inputDevice"></param>
        void OnCheckInput(InputEventPtr eventPtr, InputDevice inputDevice)
        {
            if (!eventPtr.IsA<StateEvent>() && !eventPtr.IsA<DeltaStateEvent>())
                return;

            if (inputDevice is Keyboard keyboard)
            {
                CheckKeyboard1Input(keyboard);
                CheckKeyboard2Input(keyboard);
            }

            if (inputDevice is Gamepad gamepad)
            {
                CheckGamepadInput(gamepad);
            }
        }

        /// <summary>
        /// Checks for input on the WASD keyboard setup, and stores it in a keyboard configuration accordingly
        /// </summary>
        /// <param name="keyboard"></param>
        void CheckKeyboard1Input(Keyboard keyboard)
        {
            if (keyboard.spaceKey.isPressed)
            {
                KeyboardSetup keyboardSetup = ContainsKeyboardDevice(Keyboard.current, keyboardDevices);
                    
                if(keyboardSetup == null)
                {
                    //if this keyboard device is currently not in our keyboardDevices list, then let's join player and check off the keyboard1 control scheme for it
                    KeyboardSetup newKeyboardSetup = new KeyboardSetup(Keyboard.current);
                    newKeyboardSetup.keyboard1Taken = true;
                    keyboardDevices.Add(newKeyboardSetup);
                    var p = PlayerInput.Instantiate(playerConfigurationWASDPrefab, controlScheme: "Keyboard", pairWithDevice: Keyboard.current);
                    
                    //Instantiate(playerConfigurationWASDPrefab);
                }
                else if(!keyboardSetup.keyboard1Taken)
                {
                    keyboardSetup.keyboard1Taken = true;
                    var p = PlayerInput.Instantiate(playerConfigurationWASDPrefab, controlScheme: "Keyboard", pairWithDevice: Keyboard.current);
                }
            }
        }
        
        /// <summary>
        /// Checks for input on the arrow keyboard setup, and stores it in a keyboard configuration accordingly
        /// </summary>
        /// <param name="keyboard"></param>
        void CheckKeyboard2Input(Keyboard keyboard)
        {
            if (keyboard.enterKey.isPressed)
            {
                KeyboardSetup keyboardSetup = ContainsKeyboardDevice(Keyboard.current, keyboardDevices);

                if (keyboardSetup == null)
                {
                    //if this keyboard device is currently not in our keyboardDevices list, then let's join player and check off the keyboard1 control scheme for it
                    KeyboardSetup newKeyboardSetup = new KeyboardSetup(Keyboard.current);
                    newKeyboardSetup.keyboard2Taken = true;
                    keyboardDevices.Add(newKeyboardSetup);
                    var p = PlayerInput.Instantiate(playerConfigurationArrowsPrefab, controlScheme: "Keyboard2", pairWithDevice: Keyboard.current);
                }
                else if (!keyboardSetup.keyboard2Taken)
                {
                    keyboardSetup.keyboard2Taken = true;
                    var p = PlayerInput.Instantiate(playerConfigurationArrowsPrefab, controlScheme: "Keyboard2", pairWithDevice: Keyboard.current);
                }
            }
        }
        
        /// <summary>
        /// Checks for input on the gamepad setup, and stores it in a gamepad configuration accordingly
        /// </summary>
        /// <param name="gamepad"></param>
        void CheckGamepadInput(Gamepad gamepad)
        {
            if (Gamepad.current == null) return;

            if (gamepad.buttonSouth.isPressed)
            {
                GamepadSetup gamepadDevice = ContainsGamepadDevice(Gamepad.current, gamepadDevices);

                if (gamepadDevice == null)
                {
                    gamepadDevice = new GamepadSetup(Gamepad.current);
                    gamepadDevices.Add(gamepadDevice);
                    var p = PlayerInput.Instantiate(playerConfigurationGamepadPrefab, controlScheme: "Controller", pairWithDevice: Gamepad.current);
                }
            }
        }
        
        KeyboardSetup ContainsKeyboardDevice(Keyboard keyboard, List<KeyboardSetup> keyboardSetups)
        {
            foreach (KeyboardSetup keyboardSetup in keyboardSetups)
            {
                if (keyboard == keyboardSetup.keyboard) return keyboardSetup;
            }
            return null;
        }

        GamepadSetup ContainsGamepadDevice(Gamepad gamepad, List<GamepadSetup> gamepadSetups)
        {
            foreach (GamepadSetup gamepadSetup in gamepadSetups)
            {
                if (gamepad == gamepadSetup.gamepad) return gamepadSetup;
            }
            return null;
        }

        public List<PlayerConfiguration> GetPlayerConfigs() => playerConfigs;
        
        public void SetPlayerColor(int playerIndex, Material color) => playerConfigs[playerIndex].SetPlayerColor(color);

        public void ReadyPlayer(int playerIndex) => playerConfigs[playerIndex].SetReady(true);

        public void StartLevel()
        {
            if (playerConfigs.All(p => p.IsReady))
            {
                Deinitialize();
                SceneManager.LoadScene(sceneToLoad);
            }
        }

        public void HandlePlayerJoin(PlayerInput playerInput)
        {            
            if(!playerConfigs.Any(p => p.PlayerIndex == playerInput.playerIndex))
            {
                //playerInput.transform.SetParent(transform);
                playerConfigs.Add(new PlayerConfiguration(playerInput));
            }
        }

        public void ClearPlayerConfigs()
        {
            //Destroy all player input objects and clear the list of player configs
            foreach (var playerConfig in playerConfigs)
            {
                Destroy(playerConfig.PlayerInput.gameObject);
            }
            playerConfigs.Clear();
        }

        public void ClearDevicesBeingUsed()
        {
            keyboardDevices.Clear();
            gamepadDevices.Clear();
        }
    }

    #endregion
}
