using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Designed to be applied to any scriptable object droppable objects (food, powerups, etc.)
/// - NOT THE ACTUAL PREFABS
/// </summary>
namespace Bonkers.Drops
{
    public interface IDroppableObject
    {
        void Spawn(Vector3 position);
    }
}