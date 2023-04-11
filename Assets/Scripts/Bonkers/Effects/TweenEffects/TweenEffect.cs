using UnityEngine;

namespace Bonkers.Effects
{
    public abstract class TweenEffect : ScriptableObject
    {
        public abstract void ExecuteEffect(Transform transform);
    }
}