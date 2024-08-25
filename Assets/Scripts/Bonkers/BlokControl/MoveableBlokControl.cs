using UnityEngine;
using Bonkers.Combat;
using Bonkers.Score;

namespace Bonkers.BlokControl
{
    /// <summary>
    /// Contains all common functionality that all Moveable BlokControls (BasicBlokControl, GlassBlokControl, IceBlokControl, etc) should need.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(IBlokInteraction))]
    [RequireComponent(typeof(BlokHitTracker))]
    [RequireComponent(typeof(BlokInteraction))]
    public abstract class MoveableBlokControl : BlokControl, IMoveableBlokControl
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
        protected BlokHitTracker BlokHitTrackerClass;

        #endregion

        #region Unity Events/Functions

        protected override void Awake()
        {
            base.Awake();
            BlokHitTrackerClass = GetComponent<BlokHitTracker>();
        }

        protected virtual void Start()
        {
            GameObject wallMovePointsParentGameObject = GameObject.FindGameObjectWithTag("MovePointStorage");
            
            if (!wallMovePointsParentGameObject) return;
            wallMovePointsParent = wallMovePointsParentGameObject.transform;
            SetMovePointParent();
        }

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.transform.tag == "Enemy" && isMoving)
            {
                HitEnemy(collision.transform);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            blokInteraction.OnSetMoving += SetMoving;
            blokHealth.OnBreakBlok += SetNotMoving;
            blokHealth.OnRespawnBlok += ResetMovePoint;
            blokInteraction.OnBlokBumped += BlokBumped;
            blokInteraction.OnBlokImpact += OnBlokImpact;
            blokInteraction.OnBlokDestroyInImpact += DestroyBlok;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            blokInteraction.OnSetMoving -= SetMoving;
            blokHealth.OnBreakBlok -= SetNotMoving;
            blokHealth.OnRespawnBlok -= ResetMovePoint;
            blokInteraction.OnBlokBumped -= BlokBumped;
            blokInteraction.OnBlokImpact -= OnBlokImpact;
            blokInteraction.OnBlokDestroyInImpact -= DestroyBlok;
        }

        #endregion

        #region Class Functions

        //Can be implemented in inherited classes
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

        /// <summary>
        /// This is used to reset movement variables
        /// </summary>
        /// <param name="waitTime"></param>
        private void SetNotMoving(float waitTime)
        {
            SetMoving(false, Vector3.zero);
        }
        
        public void SetMoving(bool isMoving, Vector3 moveDir)
        {
            this.isMoving = isMoving;
            this.moveDir = moveDir;
        }

        public float CurrentSpeed => moveSpeed;

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