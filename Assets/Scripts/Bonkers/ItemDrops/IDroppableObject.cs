using UnityEngine;

namespace Bonkers.ItemDrops
{
    public interface IDroppableObject
    {
        void Spawn(Vector3 position);
    }
}