using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bonkers.Combat
{
    public class BasicBlokInteraction : BlokInteraction, IBlokInteraction
    {        
        public void BlokHit(Vector3 playerFacingDirection)
        {
            Collider2D nextOverBlokCollider = Physics2D.OverlapCircle(transform.position + playerFacingDirection, checkRadius, bonkableLayers);
            if (nextOverBlokCollider)
            {
                OnBlokHit?.Invoke();
                //Invokes our event for destroying the blok, so any listeners can run their logic
                InvokeOnBlokDestroyInImpact();
                TriggerBlokImpact(true);
            }
            else
            {
                //Play bonk audio if it starts moving
                OnTriggerBonkAudio?.Invoke();
                SetMoving(true, playerFacingDirection);
            }
        }

        public void SetMoving(bool shouldMove, Vector3 playerFacingDirection)
        {
            OnSetMoving?.Invoke(shouldMove, playerFacingDirection);
        }

        public void BlokBumped(Vector3 playerFacingDirection, Vector3 currentPlayerPosition)
        {
            //Basic blok will do nothing right now when bumped by player
        }
    }
}