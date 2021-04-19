using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bonkers.Combat;
using Bonkers.Movement;
using System;

namespace Bonkers.Control
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerMovement))]
    [RequireComponent(typeof(PlayerCombat))]
    [RequireComponent(typeof(PlayerHealth))]
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(PlayerEffects))]

    public class PlayerControl : MonoBehaviour
    {
        #region Inspector and Public Variables

        [SerializeField] int playerNum = 1;
        [SerializeField] [Range(0f, 5f)] float timeBetweenBonks = 0.5f;
        [SerializeField] float waitForNewInputTime = 0.2f;
        [SerializeField] bool canTakeInput = true;
        [SerializeField] bool useInputBuffer = false;
        [SerializeField] float inputBufferTime = 2f;
        public float horizontalMovementInputBuffer = 0f;
        public float verticalMovementInputBuffer = 0f;



        #endregion

        #region Class and Cached Variables
        PlayerInput playerInput;
        PlayerMovement mover;
        PlayerCombat combat;
        PlayerEffects effects;
        Animator animator;
        bool canBonk = true;
        IEnumerator inputCancelCoroutine;
        #endregion 

        #region Unity Event Methods

        void Awake()
        {
            mover = GetComponent<PlayerMovement>();
            combat = GetComponent<PlayerCombat>();
            playerInput = GetComponent<PlayerInput>();
            effects = GetComponent<PlayerEffects>();
            animator = GetComponent<Animator>();
        }

        // Start is called before the first frame update
        void Start()
        {            
            inputCancelCoroutine = HoldHorizontalInput();
            StartCoroutine(inputCancelCoroutine);
        }

        // Update is called once per frame
        void Update()
        {
            if (useInputBuffer)
            {
                HandleInput();
                HandleMovement();
            }
            else
            {
                HandleMovementNoBuffer();
            }
            HandleBonking();
            
        }

        void OnEnable()
        {
            playerInput.checkRedirectMovement += CheckRedirectMovement;    
        }

        void OnDisable()
        {
            playerInput.checkRedirectMovement -= CheckRedirectMovement;    
        }

        void HandleMovementNoBuffer()
        {
            transform.position = Vector3.MoveTowards(transform.position, mover.GetMovePoint().position, mover.GetMoveSpeed() * Time.deltaTime);

            if (Vector3.Distance(transform.position, mover.GetMovePoint().position) <= mover.GetCheckDistance())
            {
                float inputHorizontal = playerInput.movementInputP1.x;
                float inputVertical = playerInput.movementInputP1.y;
                if (Math.Abs(inputHorizontal) == 1)
                {
                    mover.SetRotation(true, inputHorizontal);
                    if (mover.MovePointHorizontal(inputHorizontal)) return;
                }
                else if (Math.Abs(inputVertical) == 1)
                {
                    mover.SetRotation(false, inputVertical);
                    if (mover.MovePointVertical(inputVertical)) return;
                }
                else
                {
                    animator.SetBool("walking", false);
                }
            }
            else
            {
                animator.SetBool("walking", true);
            }
        }

        void CheckRedirectMovement(Vector2 newDirection)
        {
            if (-mover.facingDir.x == newDirection.x || -mover.facingDir.y == newDirection.y)
            {
                //New direction is opposite of currently facing direction, so redirect
                float inputHorizontal = newDirection.x;
                float inputVertical = newDirection.y;
                if (Math.Abs(inputHorizontal) == 1)
                {
                    mover.SetRotation(true, inputHorizontal);
                    if (mover.MovePointHorizontal(inputHorizontal)) return;
                }
                else if (Math.Abs(inputVertical) == 1)
                {
                    mover.SetRotation(false, inputVertical);
                    if (mover.MovePointVertical(inputVertical)) return;
                }
            }
        }

        #endregion

        #region Class Methods

        void HandleInput()
        {
            float inputHorizontal = Input.GetAxisRaw("Horizontal" + this.playerNum.ToString());
            if (Math.Abs(inputHorizontal) == 1 && canTakeInput)
            {
                horizontalMovementInputBuffer = inputHorizontal;
                verticalMovementInputBuffer = 0f;

                StopCoroutine(inputCancelCoroutine);
                inputCancelCoroutine = HoldHorizontalInput();
                StartCoroutine(inputCancelCoroutine);
                StartCoroutine(PauseInput());
            }

            float inputVertical = Input.GetAxisRaw("Vertical" + this.playerNum.ToString());
            if (Math.Abs(inputVertical) == 1 && canTakeInput)
            {
                verticalMovementInputBuffer = inputVertical;
                horizontalMovementInputBuffer = 0f;

                StopCoroutine(inputCancelCoroutine);
                inputCancelCoroutine = HoldVerticalInput();
                StartCoroutine(inputCancelCoroutine);
                StartCoroutine(PauseInput());
            }
        }

        IEnumerator HoldHorizontalInput()
        {
            yield return new WaitForSeconds(inputBufferTime);
            horizontalMovementInputBuffer = 0f;
        }

        IEnumerator HoldVerticalInput()
        {
            yield return new WaitForSeconds(inputBufferTime);
            verticalMovementInputBuffer = 0f;
        }

        IEnumerator PauseInput()
        {
            canTakeInput = false;
            yield return new WaitForSeconds(waitForNewInputTime);
            canTakeInput = true;        
        }

        /// <summary>
        /// This will handle the 'movePoint' transform in the mover component; will grab input
        ///     if we are at the movepoint
        ///         will check if it can move in the requested direction, either horizontal or vertical, and move the movePoint
        /// 
        /// Player movement will handle actually moving the player towards the movePoint every frame.
        /// </summary>
        void HandleMovement()
        {
            transform.position = Vector3.MoveTowards(transform.position, mover.GetMovePoint().position, mover.GetMoveSpeed() * Time.deltaTime);

            if (Vector3.Distance(transform.position, mover.GetMovePoint().position) <= mover.GetCheckDistance())
            {

                if (horizontalMovementInputBuffer != 0)
                {                    
                    mover.SetRotation(true, horizontalMovementInputBuffer);
                    if (mover.MovePointHorizontal(horizontalMovementInputBuffer)) return;
                }
                else if (verticalMovementInputBuffer != 0)
                {
                    mover.SetRotation(false, verticalMovementInputBuffer);
                    if (mover.MovePointVertical(verticalMovementInputBuffer)) return;
                }
            }
        }

        //will be called before movepoint is moved in mover
        public void ClearInputBuffers()
        {
            horizontalMovementInputBuffer = 0f;
            verticalMovementInputBuffer = 0f;                        
        }

        void HandleBonking()
        {
            if (playerNum == 1 && canBonk && playerInput.bonkInputP1)
            {
                combat.BonkBlok();
                animator.SetTrigger("bonk");
                StartCoroutine(WaitForBonk());
            }
            /**
            else if (this.playerNum == 2 && canBonk && playerInput.bonkInputP2)
            {
                combat.BonkBlok();
                StartCoroutine(WaitForBonk());
            }
            */
        }
        IEnumerator WaitForBonk()
        {
            this.canBonk = false;
            yield return new WaitForSeconds(timeBetweenBonks);
            this.canBonk = true;
        }

        public int GetPlayerNum()
        {
            return this.playerNum;
        }

        public void Boost()
        {
            if (mover.boostCooled)
            {
                mover.StartBoostEffect();
                effects.PlayBoostEffect();
            }            
        }

        public void CancelBoost()
        {
            StopBoostEffect();
        }

        public void StopBoostEffect()
        {
            effects.StopBoostEffect();
        }
        #endregion
    }
}