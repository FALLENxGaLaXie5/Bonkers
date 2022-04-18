using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bonkers.Combat
{
    public class BombBlokInteraction : BlokInteraction, IBlokInteraction
    {
        public void BlokHit(Vector3 playerFacingDirection)
        {
            SetMoving(true, playerFacingDirection);
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
