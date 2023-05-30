using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bonkers.Combat;

namespace Bonkers.BlokControl
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(IBlokInteraction))]
    public class BombBlokControl : MoveableBlokControl
    {
        #region Inspector/Public Variables

        [SerializeField] float explosionObjectLife = 5f;
        [SerializeField] int explosionDamage = 5;
        [SerializeField] float explosionCheckRadius = 0.45f;
        [SerializeField] private BombBlokExplosionObjectControl explosionObjectControl;

        public LayerMask lifeformMask;
        //public GameObject explosionObject;

        #endregion

        #region Class Variables

        List<Vector3> allDirs = new List<Vector3>();
        AudioSource explosionAudio;

        #endregion

        #region Unity Event Functions

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            allDirs.Add(Vector3.up);
            allDirs.Add(Vector3.down);
            allDirs.Add(Vector3.left);
            allDirs.Add(Vector3.right);
            explosionAudio = explosionObjectControl.GetComponent<AudioSource>();
        }

        // Update is called once per frame
        void Update()
        {
            if (!isMoving) return;

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
                    //create a copy of class variable "moveDir", since it will now be reset to stop movement
                    Vector3 moveDirCopy = this.moveDir;
                    SetMoving(false, Vector3.zero);
                        
                    //explode!
                    ExplodeOnImpact(blokCollider, moveDirCopy);
                }
            }
        }

        #endregion

        #region Class Functions

        /// <summary>
        /// Split blok into pieces and apply explosion force.
        /// Set explosion object as active/destroy it in a number of seconds.
        /// Explode cross section
        ///     -Blok in front of, and directly to sides, will either be set moving off in opposing directions OR shatter, depending on type
        ///     - Basic Bloks/Ice bloks start moving
        ///     - Wood Bloks/Glass bloks shatter
        /// </summary>
        /// <param name="impactCollider"></param>
        /// <param name="moveDirCopy"></param>
        public void ExplodeOnImpact(Collider2D impactCollider, Vector3 moveDirCopy)
        {
            //If it's already been exploded, get out of this function
            if (explosionObjectControl.Exploded) return;
            explosionObjectControl.Explode();

            if (moveDirCopy == Vector3.zero)
            {
                BlockExplodeNonMoving();
            }
            else
            {
                BlokExplodeMoving(impactCollider, moveDirCopy);
            }

            //Checks for enemy kills
            CheckForKills();
            
            //This will actually move the bomb blok itself and set it up to pool and respawn, on top of playing the explosion animation
            //Need to do this after checking any positional stuff for the bomb exploding since this moves the blok outside the play area
            ExplodeBlok();
        }

        void ExplodeBlok()
        {
            explosionObjectControl.transform.parent = null;
            explosionObjectControl.gameObject.SetActive(true);
            health.BreakBlok();
        }

        void BlokExplodeMoving(Collider2D impactCollider, Vector3 moveDirCopy)
        {
            //Bomb Blok impacts another blok after being bonked by something            
            List<Vector3> adjDirs = GetAdjacentDirections(moveDirCopy);

            //explode cross section bloks
            Collider2D adjBlok0 = Physics2D.OverlapCircle(transform.position + adjDirs[0], 0.2f, wallBonkMask);
            Collider2D adjBlok1 = Physics2D.OverlapCircle(transform.position + adjDirs[1], 0.2f, wallBonkMask);
            CheckAndExecuteBlokAction(impactCollider, moveDirCopy);
            if (adjBlok0)
                CheckAndExecuteBlokAction(adjBlok0, adjDirs[0]);
            if (adjBlok1)
                CheckAndExecuteBlokAction(adjBlok1, adjDirs[1]);
        }

        void BlockExplodeNonMoving()
        {
            //Bomb blok is not moving and explodes. Need to blow up entire 4 way cross section in this case
            foreach (Vector3 dir in this.allDirs)
            {
                Collider2D adjBlok = Physics2D.OverlapCircle(transform.position + dir, 0.2f, wallBonkMask);
                if (adjBlok)
                    CheckAndExecuteBlokAction(adjBlok, dir);
            }
        }

        void CheckForKills()
        {
            foreach (Vector3 dir in this.allDirs)
            {
                Collider2D adjRaycast = Physics2D.OverlapCircle(transform.position + dir, explosionCheckRadius, lifeformMask);
                if (adjRaycast)
                {
                    ExecuteLifeform(adjRaycast);
                }
            }
        }

        /// <summary>
        /// Checks what to do when blok explodes in cross section of either AI or Players
        /// </summary>
        void ExecuteLifeform(Collider2D hit)
        {
            IHealth health = hit.transform.GetComponent<IHealth>();
            if (health == null)
            {
                health = hit.transform.parent.GetComponent<IHealth>();
            }
            health.TakeDamage(explosionDamage);
        }

        /// <summary>
        /// Checks what to do with adj bloks in cross section explosion. Will either move or shatter in respective directions.
        /// </summary>
        /// <param name="impactCollider"></param>
        /// <param name="moveDirCopy"></param>
        void CheckAndExecuteBlokAction(Collider2D impactCollider, Vector3 moveDirCopy)
        {
            string blokTag = impactCollider.transform.tag;
            BlokHealth blokHealth = impactCollider.transform.GetComponent<BlokHealth>();
            IMoveableBlokControl blokControlInterface = impactCollider.transform.GetComponent<IMoveableBlokControl>();
            if (blokTag == "BasicBlok" || blokTag == "IceBlok")
            {
                Vector2 overlapPositionToCheck = transform.position + (moveDirCopy * 2);
                Collider2D nextOverBlokCollider = Physics2D.OverlapCircle(overlapPositionToCheck, 0.2f, wallBonkMask);
                if (nextOverBlokCollider)
                {
                    blokHealth.BreakBlok();
                }
                else
                {
                    blokControlInterface.SetMoving(true, moveDirCopy);
                }
            }
            else if (blokTag == "WoodenBlok" || blokTag == "GlassBlok")
            {
                blokHealth.BreakBlok();
            }
            else if (blokTag == "BombBlok")
            {
                impactCollider.transform.GetComponent<BombBlokControl>().ExplodeOnImpact(null, Vector3.zero);
                //blokHealth.DestroyBlok();
            }
        }

        List<Vector3> GetAdjacentDirections(Vector3 moveDirCopy)
        {
            List<Vector3> adjDirs = new List<Vector3>();
            if (moveDirCopy == Vector3.up || moveDirCopy == Vector3.down)
            {
                adjDirs.Add(Vector3.left);
                adjDirs.Add(Vector3.right);
            }
            else if (moveDirCopy == Vector3.right || moveDirCopy == Vector3.left)
            {
                adjDirs.Add(Vector3.up);
                adjDirs.Add(Vector3.down);
            }
            return adjDirs;
        }

        /*
        public void HitEnemy(Transform enemyTransform)
        {
            EnemyHealth enemyHealthClass = enemyTransform.GetComponent<EnemyHealth>();
            enemyHealthClass.TakeDamage(moveDamage);
            int enemyHitScoreValue = enemyHealthClass.GetScoreValue();
            onEnemyHit?.Invoke(enemyHitScoreValue);            
        }
        */

        
        #endregion
    }
}