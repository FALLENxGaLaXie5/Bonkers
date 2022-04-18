using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bonkers.Combat
{
    public class GlassBlokInteraction : BlokInteraction, IBlokInteraction
    {


        public void BlokHit(Vector3 playerFacingDirection)
        {
            Collider2D nextOverBlokCollider = Physics2D.OverlapCircle(transform.position + playerFacingDirection, checkRadius, bonkableLayers);
            if (nextOverBlokCollider)
            {
                OnBlokHit?.Invoke();
            }
            else
            {
                SetMoving(true, playerFacingDirection);
            }
        }

        public void SetMoving(bool shouldMove, Vector3 playerFacingDirection)
        {
            OnSetMoving?.Invoke(shouldMove, playerFacingDirection);
        }

        public void BlokBumped(Vector3 playerFacingDirection, Vector3 currentPlayerPosition)
        {
            //Nothing for now
        }
    }
}
