using System;
using DG.Tweening;
using UnityEngine;

namespace Bonkers.Effects
{
    [CreateAssetMenu(fileName = "New Fade In Effect", menuName = "TweenEffects/Create New Fade In Effect")]
    public class FadeInEffect : TweenEffect<SpriteRenderer>
    {
        public override void ExecuteEffect(SpriteRenderer spriteRenderer, Action action)
        {
            if (!spriteRenderer) return;
            Color color = spriteRenderer.color;
            // Tween the alpha value of the color
            DOTween.To(
                () => color.a, alpha => { color.a = alpha; spriteRenderer.color = color; }, 1f, speed).SetEase(ease).OnComplete(() =>
            {
                action?.Invoke();
            });;;
        }
    }
}