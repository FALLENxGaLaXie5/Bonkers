using System;
using DG.Tweening;
using UnityEngine;

namespace  Bonkers.Effects
{
    [CreateAssetMenu(fileName = "New Grow Scale Effect", menuName = "TweenEffects/Create New Grow Scale Effect")]
    public class GrowScaleEffect : TweenEffect<Transform>
    {
        public override void ExecuteEffect(Transform transform, Action action = null)
        {
            if (!transform) return;
            //link will automatically kill the tween when the gameobject is destroyed
            transform.localScale = Vector3.zero;
            transform.DOScale(1f, speed).SetEase(ease).OnComplete(() =>
            {
                action?.Invoke();
            });;
        }
    }
}