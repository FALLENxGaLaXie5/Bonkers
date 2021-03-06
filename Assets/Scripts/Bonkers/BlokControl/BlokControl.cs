using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bonkers.Combat;
using Bonkers.Score;
using Bonkers.Effects;

namespace Bonkers.BlokControl
{
    /// <summary>
    /// Contains all common functionality that all BlokControl (BasicBlokControl, GlassBlokControl, IceBlokControl, etc) should need.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(IBlokInteraction))]
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(BlokHitTracker))]
    [RequireComponent(typeof(BlokInteraction))]
    [RequireComponent(typeof(BlokHealth))]
    [RequireComponent(typeof(BlokEffects))]
    public abstract class BlokControl : MonoBehaviour, IBlokControl
    {
        #region Inspector/Public Variables

        public float moveSpeed = 15f;
        public Transform movePoint;
        public LayerMask wallBonkMask;

        #endregion

        #region Class Variables

        protected Transform wallMovePointsParent;
        protected bool isMoving = false;
        protected Vector3 moveDir = Vector3.zero;
        protected AudioSource hitSound;
        protected BlokHitTracker BlokHitTrackerClass;
        protected BlokInteraction blokInteraction;
        protected BlokHealth health;
        protected BlokEffects blokEffects;

        #endregion

        #region Unity Events/Functions

        protected virtual void Awake()
        {
            blokInteraction = GetComponent<BlokInteraction>();
            BlokHitTrackerClass = GetComponent<BlokHitTracker>();
            hitSound = GetComponentInChildren<AudioSource>();
            health = GetComponent<BlokHealth>();
            blokEffects = GetComponent<BlokEffects>();
        }

        protected virtual void Start()
        {
            wallMovePointsParent = GameObject.FindGameObjectWithTag("MovePointStorage").transform;
            SetMovePointParent();
        }

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.transform.tag == "Enemy" && isMoving)
            {
                HitEnemy(collision.transform);
            }
        }

        protected virtual void OnEnable()
        {
            blokInteraction.OnSetMoving += SetMoving;
            blokInteraction.OnBlokHit += PlaySound;
            health.OnDestroyBlok += PlayDestroySound;
            health.OnRespawnBlok += ResetMovePoint;
            blokInteraction.OnBlokBumped += BlokBumped;
            blokInteraction.OnBlokImpact += OnBlokImpact;
            blokInteraction.OnBlokDestroyInImpact += DestroyBlok;
        }

        protected virtual void OnDisable()
        {
            blokInteraction.OnSetMoving -= SetMoving;
            blokInteraction.OnBlokHit -= PlaySound;
            health.OnDestroyBlok -= PlayDestroySound;
            health.OnRespawnBlok -= ResetMovePoint;
            blokInteraction.OnBlokBumped -= BlokBumped;
            blokInteraction.OnBlokImpact -= OnBlokImpact;
            blokInteraction.OnBlokDestroyInImpact -= DestroyBlok;
        }

        #endregion

        #region Class Functions

        //needs to be implemented in inherited classes
        protected virtual void BlokBumped(Vector3 bumpDirection, Vector3 hitFromPosition)
        {

        }
        
        protected virtual void OnBlokImpact()
        {
            
        }

        protected virtual void ResetMovePoint()
        {
            movePoint.position = transform.position;
        }
        
        void SetMovePointParent()
        {
            if (wallMovePointsParent)
                movePoint.parent = wallMovePointsParent;
            else
                movePoint.parent = null;
        }

        public void SetMoving(bool isMoving, Vector3 moveDir)
        {
            this.isMoving = isMoving;
            this.moveDir = moveDir;
        }

        public float CurrentSpeed => moveSpeed;
        
        /// <summary>
        /// Play generic blok hitting sound
        /// </summary>
        /// <param name="waitTime"></param>
        protected virtual void PlaySound()
        {
            if (!hitSound.isActiveAndEnabled) return;
            hitSound.Play();
        }
        
        /// <summary>
        /// Play sound for destroying blok (can use wait time specified, but likely just want to play immediately)
        /// </summary>
        /// <param name="waitTime"></param>
        protected virtual void PlayDestroySound(float waitTime)
        {
            if (!hitSound.isActiveAndEnabled) return;
            hitSound.Play();
        }

        protected virtual void DestroyBlok()
        {
            health.DestroyBlok();
        }

        public virtual void HitEnemy(Transform enemyTransform)
        {
            //Make sure we're hitting the parent game object of the enemy
            //if (enemyTransform.parent != null ) return;

            EnemyHealth enemyHealthClass = enemyTransform.GetComponent<EnemyHealth>();
            enemyHealthClass.TakeDamage(5);
            int enemyHitScoreValue = enemyHealthClass.GetScoreValue();
            BlokHitTrackerClass.Score(enemyHitScoreValue);
        }

        public bool IsMoving() => isMoving;
        public float GetCurrentSpeed() => CurrentSpeed;

        #endregion
    }
}