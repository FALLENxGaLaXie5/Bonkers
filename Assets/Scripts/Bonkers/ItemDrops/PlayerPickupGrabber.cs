using System;
using UnityEngine;

namespace  Bonkers.ItemDrops
{
    public class PlayerPickupGrabber : MonoBehaviour
    {
        [SerializeField] float timeToDestroyObjectAfterPickup;
        
        public event Action<ScriptableObject> OnPickup;

        public void AttemptPickup(IPickupable pickupable)
        {
            ScriptableObject pickup = pickupable.AttemptPickup();
            pickupable.DestroyPickupGameObject(timeToDestroyObjectAfterPickup);
            //if a pickup was successfully returned, notify listeners and give them the pickup scriptable object
            if (pickup) OnPickup?.Invoke(pickup);
        }
    }
}