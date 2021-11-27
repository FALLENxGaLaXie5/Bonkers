using Bonkers.Effects;
using UnityEngine;
using DG.Tweening;

namespace Bonkers.BlokControl
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(IBlokInteraction))]
    public class BasicBlokControl : BlokControl
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
                    PlaySound();
                    SetMoving(false, Vector3.zero);
                    blokInteraction.TriggerBlokImpact();
                }
            }
        }       
        
        protected override void OnBlokImpact()
        {
            blokEffects.ExecuteImpactEffects(transform, BlokEffects.TypeEffects.Primary);
        }

        #endregion
    }
}