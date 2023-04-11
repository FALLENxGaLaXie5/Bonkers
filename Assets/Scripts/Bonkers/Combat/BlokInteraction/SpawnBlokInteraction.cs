using UnityEngine;

namespace Bonkers.Combat
{
    public class SpawnBlokInteraction : BlokInteraction, IBlokInteraction
    {
        public void BlokHit(Vector3 playerFacingDirection)
        {
            OnBlokHit?.Invoke();
            //Invokes our event for destroying the blok, so any listeners can run their logic
            InvokeOnBlokDestroyInImpact();
        }

        public void SetMoving(bool shouldMove, Vector3 playerFacingDirection)
        {
            //Spawn blok will not move
        }

        public void BlokBumped(Vector3 playerFacingDirection, Vector3 currentPlayerPosition)
        {
            //Spawn blok does nothing when bumped by player
        }
    }
}