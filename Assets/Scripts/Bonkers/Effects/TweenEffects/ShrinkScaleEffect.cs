using System;
using DG.Tweening;
using UnityEngine;

namespace  Bonkers.Effects
{
    [CreateAssetMenu(fileName = "New Shrink Scale Effect", menuName = "TweenEffects/Create New Shrink Scale Effect")]
    public class ShrinkScaleEffect : TweenEffect<Transform>
    {
        public override void ExecuteEffect(Transform transform, Action action)
        {
            if (!transform) return;
            //link will automatically kill the tween when the gameobject is destroyed
            transform.DOScale(0f, speed).SetEase(ease).OnComplete(() =>
            {
                action?.Invoke();
            });
        }
    }
}