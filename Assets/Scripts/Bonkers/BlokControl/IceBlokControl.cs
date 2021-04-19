using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bonkers.Combat;
using Bonkers.Core;
using Bonkers.Movement;

namespace Bonkers.BlokControl
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(IBlokInteraction))]
    public class IceBlokControl : MonoBehaviour, IBlokControl
    {
        #region Inspector/Public Variables
        [SerializeField] float speedDecrement = 2f;
        [SerializeField] float stoppingSpeed = 3f;
        [SerializeField][Tooltip("Higher number will result in slower time for blok to explode")] [Range(2, 5)] int maxHitsByTimer = 3;
        [SerializeField][Tooltip("Higher number will result in quicker time for blok to explode")][Range(0.1f, 0.5f)] float timeToDecreaseHitsByTimer = 0.2f;


        public float moveSpeed = 15f;
        public Transform movePoint;
        public LayerMask wallBonkMask;

        #endregion

        #region Class Variables

        Transform wallMovePointsParent;
        bool isMoving = false;
        Vector3 moveDir = Vector3.zero;
        float currentSpeed;
        AudioSource hitSound;
        HitBy hitByClass;
        BlokHealth health;
        int currentHitsByTimer = 0;

        #endregion

        #region Unity Events/Functions
        // Start is called before the first frame update
        void Start()
        {
            hitSound = GetComponentInChildren<AudioSource>();
            this.currentSpeed = this.moveSpeed;
            wallMovePointsParent = GameObject.FindGameObjectWithTag("MovePointStorage").transform;
            SetMovePointParent();
            hitByClass = GetComponent<HitBy>();
            health = GetComponent<BlokHealth>();
            StartCoroutine(StartTimingHits());
        }



        // Update is called once per frame
        void Update()
        {
            if (this.isMoving)
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
                        }
                    }
                }
            }
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.transform.tag == "Enemy" && this.isMoving)
            {
                HitEnemy(collision.transform);
            }
            else if (collision.transform.tag == "Player" && this.isMoving)
            {
                //switch the movement direction, but do not decrease speed if bonked a player
                IncrementCurrentHitsByTimer();
                SetMoveDir(collision);
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

        void SetMoveDir(Collider2D playerCollision)
        {
            Vector3 playerDir = playerCollision.transform.GetComponent<PlayerMovement>().GetFacingDir();

            if (playerDir == -this.moveDir)
            {
                moveDir = playerDir;
            }
            else if (playerDir == this.moveDir)
            {
                //check if player hits blok from behind or blok hits player from behind
                float distance = Vector3.Distance(transform.position + moveDir, playerCollision.transform.position);
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

        public void HitEnemy(Transform enemyTransform)
        {
            EnemyHealth enemyHealthClass = enemyTransform.GetComponent<EnemyHealth>();
            enemyHealthClass.TakeDamage(5);
            int enemyHitScoreValue = enemyHealthClass.GetScoreValue();
            hitByClass.entityHitBy.GetComponent<PlayerScore>().AddToScore(enemyHitScoreValue);
        }

        public void SetMoving(bool newIsMoving, Vector3 movementDir)
        {
            this.isMoving = newIsMoving;
            this.moveDir = movementDir;
        }

        public bool IsMoving()
        {
            return this.isMoving;
        }

        public void PlaySound()
        {
            hitSound.Play();
        }

        public float GetCurrentSpeed()
        {
            return this.currentSpeed;
        }

        #endregion
    }
}