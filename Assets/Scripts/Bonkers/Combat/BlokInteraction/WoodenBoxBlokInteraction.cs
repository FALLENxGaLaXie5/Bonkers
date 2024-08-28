using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bonkers.Combat
{
    [RequireComponent(typeof(WoodenBlokBonks))]
    public class WoodenBoxBlokInteraction : BlokInteraction, IBlokInteraction
    {
        WoodenBlokBonks bonksInstance;
        
        void Awake() => bonksInstance = GetComponent<WoodenBlokBonks>();

        public void BlokHit(Vector3 playerFacingDirection)
        {
            OnBlokHit?.Invoke();
            if (bonksInstance.IsBroken)
            {
                TriggerBlokImpact(true);
            }
            else
            {
                TriggerBlokImpact(false);
                //OnTriggerBonkAudio?.Invoke();
            }
        }

        public void SetMoving(bool shouldMove, Vector3 playerFacingDirection)
        {
            //Wooden bloks don't move silly!
        }

        public void BlokBumped(Vector3 playerFacingDirection, Vector3 currentPlayerPosition)
        {
            //Nothing for now
        }
    }
}