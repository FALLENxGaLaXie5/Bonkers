using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Bonkers.Combat;
using Bonkers.Effects;
using Bonkers.Score;
using Vector3 = UnityEngine.Vector3;

//using Bonkers.Movement;

namespace Bonkers.BlokControl
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(IBlokInteraction))]
    public class IceBlokControl : BlokControl, IBlokControl
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
            currentSpeed = moveSpeed;
            StartCoroutine(StartTimingHits());
        }



        // Update is called once per frame
        void Update()
        {
            if (isMoving)
            {
                transform.position = Vector3.MoveTowards(transform.position, movePoint.position, currentSpeed * Time.deltaTime);

                if (Vector3.Distance(transform.position, movePoint.position) <= .05f)
                {
                    Collider2D blokCollider = Physics2D.OverlapCircle(transform.position + moveDir, 0.2f, wallBonkMask);
                    if (!blokCollider)
                    {
                        movePoint.position += moveDir;
                    }
                    else
                    {
                        IncrementCurrentHitsByTimer();
                        PlaySound();
                        if (currentSpeed <= stoppingSpeed)
                        {
                            SetMoving(false, Vector3.zero);
                            currentSpeed = moveSpeed;
                        }
                        else
                        {
                            //switch the movement dir, and also decrease the speed if bonked a wall
                            SwitchMoveDir(true);
                            blokInteraction.TriggerBlokImpact();
                        }
                    }
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

        void IncrementCurrentHitsByTimer()
        {
            currentHitsByTimer++;
            if (currentHitsByTimer >= maxHitsByTimer)
            {
                health.DestroyBlok();
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
            if (bumpDirection == -this.moveDir)
            {
                moveDir = bumpDirection;
            }
            else if (bumpDirection == this.moveDir)
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
                    this.currentSpeed += speedDecrement;
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
        
        protected override void OnBlokImpact()
        {
            if (moveDir == Vector3.left || moveDir == Vector3.right)
                blokEffects.ExecuteImpactEffects(transform, BlokEffects.TypeEffects.Primary);
            if (moveDir == Vector3.up || moveDir == Vector3.down)
                blokEffects.ExecuteImpactEffects(transform, BlokEffects.TypeEffects.Alternate);
        }

        #endregion
    }
}