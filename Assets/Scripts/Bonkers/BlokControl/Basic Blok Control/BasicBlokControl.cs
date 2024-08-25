using Bonkers.Effects;
using UnityEngine;

namespace Bonkers.BlokControl
{
    [DisallowMultipleComponent]
    public class BasicBlokControl : MoveableBlokControl
    {
        #region Unity Events/Functions

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
                    SetMoving(false, Vector3.zero);
                    blokInteraction.TriggerBlokImpact(false);
                }
            }
        }       
        
        protected override void OnBlokImpact(bool destroyInImpact)
        {
            base.OnBlokImpact(destroyInImpact);
            //Do not execute impact effects if the blok is being destroyed
            if (destroyInImpact) return; 
            blokEffects.ExecuteImpactEffects(transform, BlokEffects.TypeEffects.Primary);
        }

        #endregion
    }
}