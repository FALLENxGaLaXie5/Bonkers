using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Bonkers.Control
{
    public class PlayerInput : MonoBehaviour
    {
        #region Public/Inspector Variables

        [SerializeField] float boostInputDelay = 0.2f;
        public bool canBoost = false;
        public Vector2 movementInputP1 { get; private set; }

        public bool bonkInputP1 { get; private set; }

        public event Action<Vector2> checkRedirectMovement;


        #endregion

        #region Class/Private Variables
        InputMaster controls;
        Vector2 currentBoostInput = Vector2.zero;
        IEnumerator boostCancelCoroutine;
        PlayerControl control;


        #endregion

        #region Unity Events/Functions

        void Awake()
        {
            controls = new InputMaster();
            StartCoroutine(ClearBonkInput());
            boostCancelCoroutine = BoostCancel();
        }

        void Start()
        {
            control = GetComponent<PlayerControl>();    
        }

        private void OnEnable()
        {
            controls.Player.Bonk.performed += context => Bonk();
            controls.Player.Movement.performed += context => Move(context.ReadValue<Vector2>());
            controls.Player.MovementCancel.performed += context => ClearMovement();
            controls.Player.Boost.performed += context => Boost(context.ReadValue<Vector2>());
            controls.Enable();
        }
        
        private void OnDisable()
        {
            controls.Player.Bonk.performed -= context => Bonk();
            controls.Player.Movement.performed -= context => Move(context.ReadValue<Vector2>());
            controls.Player.MovementCancel.performed -= context => ClearMovement();
            controls.Player.Boost.performed -= context => Boost(context.ReadValue<Vector2>());
            controls.Disable();
        }

        #endregion

        #region Class Functions

        IEnumerator ClearBonkInput()
        {
            while (true)
            {
                yield return null;
                this.bonkInputP1 = false;
            }
        }

        void Bonk()
        {
            bonkInputP1 = true;
        }

        void Move(Vector2 direction)
        {
            movementInputP1 = direction;
            checkRedirectMovement?.Invoke(direction);
        }

        void ClearMovement()
        {
            movementInputP1 = Vector2.zero;
            control.CancelBoost();
        }

        void Boost(Vector2 direction)
        {
            StopCoroutine(BoostCancel());
            if (direction == currentBoostInput && this.canBoost)
            {
                canBoost = false;
                //boost player in direction
                control.Boost();
            }
            else
            {
                this.currentBoostInput = direction;
                StartCoroutine(BoostCancel());
            }
            
        }

        IEnumerator BoostCancel()
        {
            canBoost = true;
            yield return new WaitForSeconds(boostInputDelay);
            canBoost = false;
            this.currentBoostInput = Vector2.zero;
        }

        #endregion
    }
}
