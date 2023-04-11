using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Bonkers.Core;
using Bonkers.Score;

namespace Bonkers.Control
{
    public class PlayerInputHandler : MonoBehaviour
    {
        #region Events/Delegates
        public event Action<Vector2> CheckRedirectMovement;
        public event Action BonkAction;
        #endregion

        #region Public/Inspector Variables

        [SerializeField] float boostInputDelay = 0.2f;
        [SerializeField] SpriteRenderer spriteRenderer;
        bool canBoost = false;
        public Vector2 movementInput { get; private set; } = Vector2.zero;

        #endregion

        #region Class/Private Variables
        Vector2 currentBoostInput = Vector2.zero;
        IEnumerator boostCancelCoroutine;
        PlayerControl playerController;
        PlayerConfiguration playerConfig;
        PlayerControls controls;


        #endregion

        #region Unity Events/Functions

        void Awake()
        {
            playerController = GetComponent<PlayerControl>();
            controls = new PlayerControls();

            boostCancelCoroutine = BoostCancel();
        }   

        #endregion

        #region Class Functions       

        public void InitializePlayer(PlayerConfiguration playerConfig, int playerNum)
        {
            this.playerConfig = playerConfig;
            spriteRenderer.color = playerConfig.PlayerColor.color;
            playerConfig.PlayerInput.onActionTriggered += PlayerInput_onActionTriggered;

            GameObject scoreObject = FindObjectOfType<PauseMenu>().transform.GetChild(playerNum - 1).gameObject;
            scoreObject.SetActive(true);

            //Scorer scorer = FindObjectsOfType<Scorer>()[playerNum - 1];
            //scorer.transform.parent.gameObject.SetActive(true);
            GetComponent<PlayerScore>().SetupScoreUI(scoreObject.GetComponentInChildren<Scorer>().gameObject);
        }

        public void DestroyInputConfiguration()
        {
            Debug.Log("GOT THE INPUTTT!!!");
            Destroy(playerConfig.PlayerInput.gameObject);
        }

        void PlayerInput_onActionTriggered(InputAction.CallbackContext context)
        {
            if (context.action.name == controls.Player.Movement.name) Move(context);
            else if (context.action.name == controls.Player.Bonk.name) Bonk(context);
            else if (context.action.name == controls.Player.MovementCancel.name) ClearMovement(context);
            else if (context.action.name == controls.Player.Boost.name) Boost(context);
        }

        public void Move(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            movementInput = context.ReadValue<Vector2>();
            CheckRedirectMovement?.Invoke(movementInput);
        }

        public void Bonk(InputAction.CallbackContext context)
        {
            if (!context.action.triggered) return;
            //PlayerController will be listening for a bonk action
            BonkAction?.Invoke();
        }

        public void ClearMovement(InputAction.CallbackContext context)
        {
            if (!playerController) return;

            if (!context.performed) return;
            movementInput = Vector2.zero;
            playerController.CancelBoost();
        }

        public void Boost(InputAction.CallbackContext context)
        {
            if (playerConfig == null) return;
            if (!context.action.triggered) return;
            Vector2 direction = context.ReadValue<Vector2>();
            StopCoroutine(BoostCancel());
            if (direction == currentBoostInput && canBoost)
            {
                canBoost = false;
                //boost player in direction
                playerController.Boost();
            }
            else
            {
                currentBoostInput = direction;
                StartCoroutine(BoostCancel());
            }
        }
                
        IEnumerator BoostCancel()
        {
            canBoost = true;
            yield return new WaitForSeconds(boostInputDelay);
            canBoost = false;
            currentBoostInput = Vector2.zero;
        }                        
        
        #endregion
    }
}
