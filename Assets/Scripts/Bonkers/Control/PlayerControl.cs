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
        PlayerMovement playerMover;
        PlayerCombat playerCombat;
        PlayerHealth playerHealth;
        PlayerEffects playerEffects;
        PlayerScore playerScorer;
        Animator animator;
        bool canBonk = true;
        IEnumerator inputCancelCoroutine;
        #endregion

        #region Events
        public event Action<PlayerControl> OnPlayerDestroy;

        #endregion

        #region Unity Event Methods

        void Awake()
        {
            playerMover = GetComponent<PlayerMovement>();
            playerCombat = GetComponent<PlayerCombat>();
            playerEffects = GetComponent<PlayerEffects>();
            animator = GetComponent<Animator>();
            playerInput = GetComponent<PlayerInputHandler>();
            playerHealth = GetComponent<PlayerHealth>();
            playerScorer = GetComponent<PlayerScore>();
        }
        
        // Update is called once per frame
        void Update()
        {
            HandleMovement();
        }

        void OnEnable()
        {            
            playerMover.ClearInputBuffers += ClearInputBuffers;
            playerMover.StopBoostEffect += StopBoostEffect;
            playerMover.onFacingDirectionChanged += NotifyAllOfChangeDirection;
            playerInput.CheckRedirectMovement += CheckRedirectMovement;
            playerInput.BonkAction += HandleBonk;
            playerHealth.onPlayerDeath += HandleDeath;
            playerHealth.onPlayerDeath += playerInput.DestroyInputConfiguration;
            playerHealth.onPlayerDeath += playerMover.DestroyBoostBar;
            playerCombat.onHitScoreableObject += HandleHitScoreableObject;
        }

        void OnDisable()
        {            
            playerMover.ClearInputBuffers -= ClearInputBuffers;
            playerMover.StopBoostEffect -= StopBoostEffect;
            playerMover.onFacingDirectionChanged -= NotifyAllOfChangeDirection;
            playerInput.CheckRedirectMovement -= CheckRedirectMovement;
            playerInput.BonkAction -= HandleBonk;
            playerHealth.onPlayerDeath -= HandleDeath;
            playerHealth.onPlayerDeath -= playerInput.DestroyInputConfiguration;
            playerHealth.onPlayerDeath -= playerMover.DestroyBoostBar;
            playerCombat.onHitScoreableObject -= HandleHitScoreableObject;
        }

        void HandleHitScoreableObject(int scoreValue)
        {
            playerScorer.AddToScore(scoreValue);
        }

        void HandleDeath()
        {
            OnPlayerDestroy?.Invoke(this);
        }

        void NotifyAllOfChangeDirection(Vector3 newFacingDirection)
        {
            playerCombat.SetFacingDirection(newFacingDirection);
        }

        void HandleMovement()
        {
            if (!playerMover) return;
            transform.position = Vector3.MoveTowards(transform.position, playerMover.GetMovePoint().position, playerMover.GetMoveSpeed() * Time.deltaTime);

            if (Vector3.Distance(transform.position, playerMover.GetMovePoint().position) <= playerMover.GetCheckDistance())
            {
                float inputHorizontal = playerInput.movementInput.x;
                float inputVertical = playerInput.movementInput.y;
                if (Math.Abs(inputHorizontal) == 1)
                {
                    playerMover.SetRotation(true, inputHorizontal);
                    if (playerMover.MovePointHorizontal(inputHorizontal)) return;
                }
                else if (Math.Abs(inputVertical) == 1)
                {
                    playerMover.SetRotation(false, inputVertical);
                    if (playerMover.MovePointVertical(inputVertical)) return;
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
            if(((playerMover.GetFacingDir() == Vector3.left || playerMover.GetFacingDir() == Vector3.right) && -playerMover.GetFacingDir().x == newDirection.x) 
                || (playerMover.GetFacingDir() == Vector3.up || playerMover.GetFacingDir() == Vector3.down) && -playerMover.GetFacingDir().y == newDirection.y)
            {
                //New direction is opposite of currently facing direction, so redirect
                float inputHorizontal = newDirection.x;
                float inputVertical = newDirection.y;
                if (Math.Abs(inputHorizontal) == 1)
                {
                    playerMover.SetRotation(true, inputHorizontal);
                    if (playerMover.MovePointHorizontal(inputHorizontal)) return;
                }
                else if (Math.Abs(inputVertical) == 1)
                {
                    playerMover.SetRotation(false, inputVertical);
                    if (playerMover.MovePointVertical(inputVertical)) return;
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
                playerCombat.AttemptBonkBlok();
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
            if (playerMover.boostCooled)
            {
                playerMover.StartBoostEffect();
                playerEffects.PlayBoostEffect();
            }            
        }

        public void CancelBoost()
        {
            StopBoostEffect();
        }

        public void StopBoostEffect()
        {
            playerEffects.StopBoostEffect();
        }
        
        #endregion
    }
}