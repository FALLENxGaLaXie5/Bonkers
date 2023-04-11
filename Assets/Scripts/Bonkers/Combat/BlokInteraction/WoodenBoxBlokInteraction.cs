using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bonkers.Combat
{
    [RequireComponent(typeof(WoodenBlokBonks))]
    public class WoodenBoxBlokInteraction : BlokInteraction, IBlokInteraction
    {
        WoodenBlokBonks bonksInstance;
        
        void Awake()
        {
            bonksInstance = GetComponent<WoodenBlokBonks>();
        }

        public void BlokHit(Vector3 playerFacingDirection)
        {
            OnBlokHit?.Invoke();
            bonksInstance.IncrementNumTimesBonked();
            if (bonksInstance.NumTimesBonked >= bonksInstance.NumberBonksToBreak)
            {

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