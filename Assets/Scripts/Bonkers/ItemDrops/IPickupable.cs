using UnityEngine;

namespace  Bonkers.ItemDrops
{
    public interface IPickupable
    {
        ScriptableObject AttemptPickup();
        void DestroyPickupGameObject(float timeToDestroy);
    }
}