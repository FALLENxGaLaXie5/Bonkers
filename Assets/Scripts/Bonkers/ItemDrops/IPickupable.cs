using UnityEngine;

namespace  Bonkers.Drops
{
    public interface IPickupable
    {
        ScriptableObject AttemptPickup();
        void DestroyPickupGameObject(float timeToDestroy);
    }
}