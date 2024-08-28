using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Bonkers.Combat;
using Bonkers.Effects;
using Bonkers.Score;
using Unity.Profiling.LowLevel.Unsafe;
using Vector3 = UnityEngine.Vector3;

namespace Bonkers.BlokControl
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(IBlokInteraction))]
    public class IceBlokControl : MoveableBlokControl, IMoveableBlokControl
    {
        #region Inspector/Public Variables
        [SerializeField] float speedDecrement = 2f;
        [SerializeField] float stoppingSpeed = 3f;
        [SerializeField][Tooltip("Higher number will result in slower time for blok to explode")] [Range(2, 5)] int maxHitsByTimer = 3;
        [SerializeField][Tooltip("Higher number will result in quicker time for blok to explode")][Range(0.1f, 0.5f)] float timeToDecreaseHitsByTimer = 0.2f;

        #endregion

        #region Class Variables

        float currentSpeed;
        int currentHitsByTimer = 0;

        #endregion

        #region Unity Events/Functions
        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            ResetIceBlokMovementLogic(0f);
            RestartTimingHits();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            blokHealth.OnBreakBlok += ResetIceBlokMovementLogic;
            blokHealth.OnRespawnBlok += RestartTimingHits;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            blokHealth.OnBreakBlok -= ResetIceBlokMovementLogic;
            blokHealth.OnRespawnBlok -= RestartTimingHits;
        }

        // Update is called once per frame
        void Update()
        {
            if (!isMoving) return;
            transform.position = Vector3.MoveTowards(transform.position, movePoint.position, currentSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, movePoint.position) > .05f) return;
            
            Collider2D blokCollider = Physics2D.OverlapCircle(transform.position + moveDir, 0.2f, wallBonkMask);
            if (!blokCollider)
            {
                movePoint.position += moveDir;
            }
            else
            {
                IncrementCurrentHitsByTimer();
                if (currentSpeed <= stoppingSpeed)
                {
                    SetMoving(false, Vector3.zero);
                    currentSpeed = moveSpeed;
                }
                else
                {
                    //switch the movement dir, and also decrease the speed if bonked a wall
                    SwitchMoveDir(true);
                    blokInteraction.TriggerBlokImpact(false);
                }
            }
            
        }

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.transform.tag == "Enemy" && isMoving)
            {
                HitEnemy(collision.transform);
            }
            else if (collision.transform.tag == "Player" && isMoving)
            {
                //switch the movement direction, but do not decrease speed if bonked a player
                IncrementCurrentHitsByTimer();
                //SwitchMoveDir(false);
            }
        }

        #endregion

        #region Class Functions
        
        private void RestartTimingHits()
        {
            StartCoroutine(StartTimingHits());
        }

        private void ResetIceBlokMovementLogic(float waitTime)
        {
            currentSpeed = moveSpeed;
            currentHitsByTimer = 0;
        }
        
        private void IncrementCurrentHitsByTimer()
        {
            currentHitsByTimer++;
            if (currentHitsByTimer >= maxHitsByTimer)
            {
                blokHealth.BreakBlok();
            }
        }

        IEnumerator StartTimingHits()
        {
            while (true)
            {
                yield return new WaitForSeconds(timeToDecreaseHitsByTimer);
                if (currentHitsByTimer > 0) currentHitsByTimer--;
            }
        }

        protected override void BlokBumped(Vector3 bumpDirection, Vector3 bumpedFromPosition)
        {
            base.BlokBumped(bumpDirection, bumpedFromPosition);
            SetMoveDir(bumpDirection, bumpedFromPosition);
        }

        void SetMoveDir(Vector3 bumpDirection, Vector3 bumpedFromPosition)
        {
            if (bumpDirection == -moveDir)
            {
                moveDir = bumpDirection;
            }
            else if (bumpDirection == moveDir)
            {
                //check if player hits blok from behind or blok hits player from behind
                float distance = Vector3.Distance(transform.position + moveDir, bumpedFromPosition);
                if (distance < 1f)
                {
                    //blok is hitting player from behind
                    moveDir = -moveDir;
                }
                else
                {
                    //player is hitting blok from behind
                    currentSpeed += speedDecrement;
                }
            }
            else
            {
                //player facing perpendicular to blok movement direction
                moveDir = -moveDir;
            }
            //if player is hitting blok at same dir as blok is moving, do not change dir
            movePoint.position = movePoint.position + moveDir;
        }

        void SwitchMoveDir(bool decreaseSpeed)
        {
            moveDir = -moveDir;
            movePoint.position = movePoint.position + moveDir;
            if (decreaseSpeed)
            {
                currentSpeed -= speedDecrement;
            }
        }

        void SetMovePointParent()
        {
            if (wallMovePointsParent)
            {
                movePoint.parent = wallMovePointsParent;
            }
            else
            {
                movePoint.parent = null;
            }
        }
        
        protected override void OnBlokImpact(bool destroyInImpact)
        {
            base.OnBlokImpact(destroyInImpact);
            if (destroyInImpact) return;
            if (moveDir == Vector3.left || moveDir == Vector3.right)
                blokEffects.ExecuteImpactEffects(transform, BlokEffects.TypeEffects.Primary);
            if (moveDir == Vector3.up || moveDir == Vector3.down)
                blokEffects.ExecuteImpactEffects(transform, BlokEffects.TypeEffects.Alternate);
        }

        #endregion
    }
}