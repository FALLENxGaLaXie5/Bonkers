using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bonkers.Core;
using Bonkers.Combat;


namespace Bonkers.BlokControl
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(IBlokInteraction))]
    public class BasicBlokControl : MonoBehaviour, IBlokControl
    {
        #region Inspector/Public Variables

        public float moveSpeed = 15f;
        public Transform movePoint;
        public LayerMask wallBonkMask;

        #endregion

        #region Class Variables

        Transform wallMovePointsParent;
        bool isMoving = false;
        Vector3 moveDir = Vector3.zero;
        AudioSource hitSound;
        HitBy hitByClass;

        #endregion

        #region Unity Events/Functions
        // Start is called before the first frame update
        void Start()
        {
            hitSound = GetComponentInChildren<AudioSource>();
            wallMovePointsParent = GameObject.FindGameObjectWithTag("MovePointStorage").transform;
            hitByClass = GetComponent<HitBy>();
            SetMovePointParent();
        }
        
        // Update is called once per frame
        void Update()
        {
            if (this.isMoving)
            {
                transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);

                if (Vector3.Distance(transform.position, movePoint.position) <= .05f)
                {
                    Collider2D blokCollider = Physics2D.OverlapCircle(transform.position + moveDir, 0.2f, wallBonkMask);
                    if (!blokCollider)
                    {
                        movePoint.position += moveDir;
                    }
                    else
                    {
                        PlaySound();
                        SetMoving(false, Vector3.zero);
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
        }

        #endregion

        #region Class Functions

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
            //Make sure we're hitting the parent game object of the enemy
            //if (enemyTransform.parent != null ) return;

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

        public float GetCurrentSpeed()
        {
            return this.moveSpeed;
        }

        public bool IsMoving()
        {
            return this.isMoving;
        }

        public void PlaySound()
        {
            hitSound.Play();
        }

        #endregion
    }
}