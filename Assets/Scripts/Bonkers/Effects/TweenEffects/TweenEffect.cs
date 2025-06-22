using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Bonkers.Effects
{
    public abstract class TweenEffect<T> : ScriptableObject where T : Component
    {
        [SerializeField] protected float speed = 2f;
        [SerializeField, EnumToggleButtons] protected Ease ease = Ease.Linear;
        public abstract void ExecuteEffect(T component, Action action = null);

        /// <summary>
        /// Stops or reverts the effect. Override this method only if your effect needs to be explicitly stopped or reverted.
        /// Default implementation does nothing, which is appropriate for one-shot effects.
        /// </summary>
        public virtual void StopEffect(T component, Action action = null)
        {
            
        }
    }
}