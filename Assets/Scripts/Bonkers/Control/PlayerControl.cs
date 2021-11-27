using System.Collections;
using UnityEngine;
using Bonkers.Combat;
using Bonkers.Movement;
using Bonkers.SceneManagement;
using Bonkers.Score;
using System;
using Sirenix.OdinInspector;

namespace Bonkers.Control
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerEnvironmentEffectorsControl))]
    [RequireComponent(typeof(PlayerPowerupControl))]
    [RequireComponent(typeof(PlayerMovement))]
    [RequireComponent(typeof(PlayerCombat))]
    [RequireComponent(typeof(PlayerHealth))]
    [RequireComponent(typeof(PlayerEffects))]
    [RequireComponent(typeof(PlayerInputHandler))]
    [RequireComponent(typeof(PlayerScore))]

    public class PlayerControl : MonoBehaviour
    {
        #region Inspector and Public Variables

        [Title("General Player Data")]
        [SerializeField] [Range(0f, 5f)] float timeBetweenBonks = 0.5f;

        #endregion

        #region Class and Cached Variables
        PlayerInputHandler playerInput;
        PlayerMovement mover;
        PlayerCombat combat;
        PlayerHealth health;
        PlayerEffects effects;
        PlayerScore scorer;
        Animator animator;
        bool canBonk = true;
        IEnumerator inputCancelCoroutine;
        #endregion 

        #region Unity Event Methods

        void Awake()
        {
            mover = GetComponent<PlayerMovement>();
            combat = GetComponent<PlayerCombat>();
            effects = GetComponent<PlayerEffects>();
            animator = GetComponent<Animator>();
            playerInput = GetComponent<PlayerInputHandler>();
            health = GetComponent<PlayerHealth>();
            scorer = GetComponent<PlayerScore>();
        }
        
        // Update is called once per frame
        void Update()
        {
            HandleMovement();
        }

        void OnEnable()
        {            
            mover.ClearInputBuffers += ClearInputBuffers;
            mover.StopBoostEffect += StopBoostEffect;
            mover.onFacingDirectionChanged += NotifyAllOfChangeDirection;
            playerInput.CheckRedirectMovement += CheckRedirectMovement;
            playerInput.BonkAction += HandleBonk;
            health.onPlayerDeath += HandleDeath;
            combat.onHitScoreableObject += HandleHitScoreableObject;
        }

        void OnDisable()
        {            
            mover.ClearInputBuffers -= ClearInputBuffers;
            mover.StopBoostEffect -= StopBoostEffect;
            mover.onFacingDirectionChanged -= NotifyAllOfChangeDirection;
            playerInput.CheckRedirectMovement -= CheckRedirectMovement;
            playerInput.BonkAction -= HandleBonk;
            health.onPlayerDeath -= HandleDeath;
            combat.onHitScoreableObject -= HandleHitScoreableObject;
        }

        void HandleHitScoreableObject(int scoreValue)
        {
            scorer.AddToScore(scoreValue);
        }

        void HandleDeath()
        {
            FindObjectOfType<Portal>().StartTransition(0);
        }

        void NotifyAllOfChangeDirection(Vector3 newFacingDirection)
        {
            combat.SetFacingDirection(newFacingDirection);
        }

        void HandleMovement()
        {
            if (!mover) return;
            transform.position = Vector3.MoveTowards(transform.position, mover.GetMovePoint().position, mover.GetMoveSpeed() * Time.deltaTime);

            if (Vector3.Distance(transform.position, mover.GetMovePoint().position) <= mover.GetCheckDistance())
            {
                float inputHorizontal = playerInput.movementInput.x;
                float inputVertical = playerInput.movementInput.y;
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
            if(((mover.GetFacingDir() == Vector3.left || mover.GetFacingDir() == Vector3.right) && -mover.GetFacingDir().x == newDirection.x) 
                || (mover.GetFacingDir() == Vector3.up || mover.GetFacingDir() == Vector3.down) && -mover.GetFacingDir().y == newDirection.y)
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

        //will be called before movepoint is moved in mover
        void ClearInputBuffers()
        {
                               
        }

        void HandleBonk()
        {
            if(canBonk)
            {
                combat.AttemptBonkBlok();
                animator.SetTrigger("bonk");
                StartCoroutine(WaitForBonk());
            }
        }

        IEnumerator WaitForBonk()
        {
            canBonk = false;
            yield return new WaitForSeconds(timeBetweenBonks);
            canBonk = true;
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