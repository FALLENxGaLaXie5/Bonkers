using System;
using UnityEngine;

namespace  Bonkers.Drops
{
    public class PlayerPickupGrabber : MonoBehaviour
    {
        [SerializeField] float timeToDestroyObjectAfterPickup = 0f;
        
        public event Action<ScriptableObject> onPickup;

        public void AttemptPickup(IPickupable pickupable)
        {
            ScriptableObject pickup = pickupable.AttemptPickup();
            pickupable.DestroyPickupGameObject(timeToDestroyObjectAfterPickup);
            //if a pickup was successfully returned, notify listeners and give them the pickup scriptable object
            if (pickup) onPickup?.Invoke(pickup);
        }
    }
}