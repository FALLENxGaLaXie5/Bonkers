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
    }
}