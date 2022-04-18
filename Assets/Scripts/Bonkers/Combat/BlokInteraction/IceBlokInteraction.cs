using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bonkers.Combat
{
    public class IceBlokInteraction : BlokInteraction, IBlokInteraction
    {        
        public void BlokHit(Vector3 playerFacingDirection)
        {
            Collider2D nextOverBlokCollider = Physics2D.OverlapCircle(transform.position + playerFacingDirection, checkRadius, bonkableLayers);
            if (nextOverBlokCollider)
            {
                OnBlokHit?.Invoke();
                Collider2D blokCollider = GetComponent<Collider2D>();
                Transform audioSourceTransform = GetComponentInChildren<AudioSource>().transform;
                audioSourceTransform.parent = null;
                Destroy(audioSourceTransform.gameObject, 1f);
                blokCollider.transform.GetComponent<ExplodeOnOrder>().ExplodeBlok();
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
            OnBlokBumped?.Invoke(playerFacingDirection, currentPlayerPosition);
        }
    }
}