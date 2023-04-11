using UnityEngine;
using System;
using Bonkers.Score;

namespace Bonkers.Combat
{
    [DisallowMultipleComponent]
    public class PlayerCombat : MonoBehaviour
    {
        #region Inspector Variables
        public LayerMask bonkableLayers;
        [SerializeField] [Range(1.0f, 2.0f)] float bonkableDistance = 1.2f;
        #endregion

        #region Events/Delegates

        public event Action<int> onHitScoreableObject;

        #endregion

        #region Class Variables

        Vector3 facingDirection = Vector3.down;

        #endregion

        #region Class Functions

        public Vector3 FacingDirection => facingDirection;
        public void SetFacingDirection(Vector3 facingDirection) => this.facingDirection = facingDirection;

        public void AttemptBonkBlok()
        {
            Collider2D blokCollider = Physics2D.OverlapCircle(transform.position + facingDirection, 0.2f, bonkableLayers);

            //if no hit, return
            if (!blokCollider) return;
            //if not in range of blok that was hit, return
            if (Vector3.Distance(transform.position, blokCollider.transform.position) > bonkableDistance) return;

            if (blokCollider.transform.TryGetComponent(out IBlokInteraction blokInteraction))
            {
                blokInteraction.BlokHit(facingDirection);
                WaitForScoreCallback(blokCollider);
            }
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out IBlokInteraction blokInteraction))
            {
                blokInteraction.BlokBumped(facingDirection, transform.position);
            }
        }

        #endregion

        #region Class Functions

        /// <summary>
        /// When the player "bonks" a moveable blok, this function will look for a "BlokScorer" component on it.
        /// If there is a Blok Scorer component on the blok we hit, then send it a callback delegate that will be invoked if that blok hits something
        ///     (an enemy in this case) that warrants a score.
        /// </summary>
        /// <param name="blokCollider"></param>
        void WaitForScoreCallback(Collider2D blokCollider)
        {
            if (blokCollider.transform.TryGetComponent(out BlokHitTracker blokHitTracker))
            {
                blokHitTracker.SetHitCallback(NotifyHitScoreableObject);
            }
        }

        /// <summary>
        /// Notifies any listeners that we have hit a scoreable object. (ex: we bonked a blok that then hit an enemy)
        /// </summary>
        /// <param name="enemyHitScoreValue"></param>
        void NotifyHitScoreableObject(int enemyHitScoreValue)
        {
            onHitScoreableObject?.Invoke(enemyHitScoreValue);
        }

        #endregion
    }
}