using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Bonkers.Drops;
using Bonkers.Movement;

namespace Bonkers.Control
{
    public class GhostlyGrubberControl : AIControl
    {
        #region Public/Inspector Variabless

        [SerializeField][Tooltip("Min possible time interval for Ghost to transition")] float minGhostInterval = 2f;
        [SerializeField][Tooltip("Max possible time interval for Ghost to transition")] float maxGhostInterval = 10f;
        [SerializeField][Range(0.2f, 3f)][Tooltip("Time it takes for Ghostly Grubber to transition from an ethereal state to physical state and vice versa")] 
        float ghostTransitionTime = 1f;
        [SerializeField] [Range(0.05f, 0.7f)] float ghostlyAlpha = 0.3f;
        [SerializeField] [Range(0.1f, 0.6f)] float validTransitionCheckRadius = 0.4f;

        public LayerMask cannotPatrolMaskGhost = new LayerMask();
        public LayerMask validTransitionMask = new LayerMask();

        #endregion

        #region Private/Class Variables

        public bool isGhosted = false;

        AISingleSpaceMovement moverSingleSpace;
        LayerMask copyCannotPatrolMask;

        #endregion


        protected override void Start()
        {
            base.Start();
            moverSingleSpace = aiMovement as AISingleSpaceMovement;
            copyCannotPatrolMask = moverSingleSpace.cannotPatrolMask;
            StartCoroutine(WaitTransition());
        }

        IEnumerator WaitTransition()
        {
            while (true)
            {
                float ghostInterval = Random.Range(minGhostInterval, maxGhostInterval);
                yield return new WaitForSeconds(ghostInterval);
                while (!ValidGhostingLocation())
                {
                    //will keep looping every frame until at a valid ghosting location
                    yield return null;
                }
                //go in or out of ghost mode
                TransitionGhost();
            }
        }
        
        bool ValidGhostingLocation()
        {
            
            Collider2D collision = Physics2D.OverlapCircle(new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y)), validTransitionCheckRadius, validTransitionMask);
            Collider2D collisionMovePoint = Physics2D.OverlapCircle(moverSingleSpace.GetMovePoint().position, validTransitionCheckRadius, validTransitionMask);

            if (isGhosted && (collision || collisionMovePoint))
            {
                return false;
            }
            else if (!isGhosted)
            {
                return true;
            }

            return true;
            //if (!collision) return true;
            //else return false;
        }

        void TransitionGhost()
        {
            isGhosted = !isGhosted;
            if (isGhosted)
            {
                spriteRenderer.DOFade(ghostlyAlpha, ghostTransitionTime);   
                //Need to make it so ghost can walk through walls now
                moverSingleSpace.cannotPatrolMask = cannotPatrolMaskGhost;
            }
            else
            {
                spriteRenderer.DOFade(1, ghostTransitionTime);
                moverSingleSpace.cannotPatrolMask = copyCannotPatrolMask;
            }
        }
    }
}