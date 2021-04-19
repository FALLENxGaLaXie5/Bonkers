using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bonkers.Control;

namespace Bonkers.Movement
{
    [DisallowMultipleComponent]
    public class PlayerMovement : MonoBehaviour
    {
        #region InspectorVariables
        [Header("General")]
        [Tooltip("How fast player should move.")][Range(2f, 10f)][SerializeField] float moveSpeed = 5f;
        [SerializeField] float moverCheckDistance = 0.02f;
        public LayerMask whatStopsMovement;

        [Space(5)]

        [Header("Boost Variables")]
        [Tooltip("Added maximum boost effect speed when it is fully charged.")] [Range(1f, 10f)] [SerializeField] float boostEffectSpeed = 3f;
        [Tooltip("How far offset from player on the Y Axis Boost Bar will be.")][SerializeField] [Range(0.1f, 3f)] float boostBarYOffset = 1f;
        [SerializeField] [Range(0.001f, 0.1f)] float boostCoolInterval = 0.005f;
        [SerializeField] [Range(0.001f, 0.1f)] float boostCoolPerInterval = 0.01f;
        [SerializeField] float speedDepreciation = 0.1f;
        [SerializeField] float speedDepreciationTime = 0.05f;
        [SerializeField] BoostBar boostBar;

        [Space(5)]
        
        [Header("Debugging")]
        public Vector3 facingDir = Vector3.down;

        #endregion

        #region Class and Cached Variables
        public bool boostCooled { get; private set; } = true;
        float boostAvailable;
        float baseSpeed;
        Transform movePoint;
        PlayerControl playerControl;

        IEnumerator slowDownCoroutine;
        IEnumerator coolBoostCoroutine;
        #endregion

        #region Unity Event Functions

        void Start()
        {
            baseSpeed = moveSpeed;
            boostAvailable = boostEffectSpeed;
            movePoint = transform.Find("MovePoint");
            movePoint.parent = null;
            playerControl = GetComponent<PlayerControl>();
            slowDownCoroutine = SlowDown();
            coolBoostCoroutine = CoolBoost();
            boostBar.SetMaxBoost(boostEffectSpeed);
            boostBar.transform.parent.transform.parent = null;
        }

        void Update()
        {
            boostBar.transform.parent.transform.position = transform.position + new Vector3(0, boostBarYOffset, 0);
        }

        #endregion

        #region Class Functions

        public bool MovePointHorizontal(float inputHorizontal)
        {
            playerControl.ClearInputBuffers();
            Collider2D collider = Physics2D.OverlapCircle(movePoint.position + new Vector3(inputHorizontal, 0f, 0f), 0.2f, whatStopsMovement);
            if (CanMove(collider))
            {
                movePoint.position += new Vector3(inputHorizontal, 0f, 0f);
                return true;
            }
            return false;
        }

        public bool MovePointVertical(float inputVertical)
        {
            playerControl.ClearInputBuffers();
            Collider2D collider = Physics2D.OverlapCircle(movePoint.position + new Vector3(0f, inputVertical, 0f), 0.2f, whatStopsMovement);
            if (CanMove(collider))
            {
                movePoint.position += new Vector3(0f, inputVertical, 0f);
                return true;
            }
            return false;
        }

        bool CanMove(Collider2D collider)
        {
            if (!collider) return true;
            IBlokControl blokControl = collider.transform.GetComponent<IBlokControl>();
            if (blokControl.IsMoving() && blokControl.GetCurrentSpeed() >= this.GetMoveSpeed())
            {
                return true;
            }
            return false;
        }

        public void SetRotation(bool isHorizontal, float input)
        {
            if (input > 0)
            {
                if (isHorizontal) // its horizontal input going to the right
                {
                    transform.rotation = Quaternion.Euler(0, 0, 90);
                    SetFacingDir(Vector3.right);
                }
                else //its vertical input going up
                {
                    transform.rotation = Quaternion.Euler(0, 0, 180);
                    SetFacingDir(Vector3.up);
                }
            }
            else if (input < 0)
            {
                if (isHorizontal) //its horizontal input going to the left
                {
                    transform.rotation = Quaternion.Euler(0, 0, 270);
                    SetFacingDir(Vector3.left);
                }
                else // its vertical input going down
                {
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                    SetFacingDir(Vector3.down);
                }
            }
            
        }

        public Vector3 GetFacingDir()
        {
            return facingDir;
        }

        public void SetFacingDir(Vector3 newFacingDir)
        {
            this.facingDir = newFacingDir;
        }

        public float GetMoveSpeed()
        {
            return moveSpeed;
        }        

        public void SetMoveSpeed(float newSpeed)
        {
            moveSpeed = Mathf.Clamp(newSpeed, baseSpeed, baseSpeed + boostEffectSpeed);
        }

        public float GetBoostSpeed()
        {
            return boostEffectSpeed;
        }

        public void ResetMoveSpeed()
        {
            this.moveSpeed = this.baseSpeed;
        }

        public Transform GetMovePoint()
        {
            return this.movePoint;
        }

        public float GetCheckDistance()
        {
            return this.moverCheckDistance;
        }

        public void StartBoostEffect()
        {
            SetMoveSpeed(moveSpeed + boostAvailable);

            StopCoroutine(slowDownCoroutine);
            StopCoroutine(coolBoostCoroutine);

            slowDownCoroutine = SlowDown();
            coolBoostCoroutine = CoolBoost();
            StartCoroutine(slowDownCoroutine);
            StartCoroutine(coolBoostCoroutine);                   
        }

        public void StaminaEffect(float effectIncrease)
        {
            boostCoolPerInterval += effectIncrease;
        }

        public void StopStaminaEffect(float effectDecrease)
        {
            boostCoolPerInterval -= effectDecrease;
        }

        IEnumerator SlowDown()
        {
            while (moveSpeed > baseSpeed)
            {
                yield return new WaitForSeconds(speedDepreciationTime);
                SetMoveSpeed(moveSpeed - speedDepreciation);
            }            
            playerControl.StopBoostEffect();
        }

        IEnumerator CoolBoost()
        {
            boostAvailable = 0f;
            boostBar.SetBoost(boostAvailable);
            while (boostAvailable < boostEffectSpeed)
            {
                yield return new WaitForSeconds(boostCoolInterval);
                boostAvailable += boostCoolPerInterval;
                boostBar.SetBoost(boostAvailable);
            }            
        }

        

        #endregion
    }
}
