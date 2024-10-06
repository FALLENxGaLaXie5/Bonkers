using System;
using DG.Tweening;
using UnityEngine;

namespace Bonkers.Effects
{
    [CreateAssetMenu(fileName = "New Fade Out Effect", menuName = "TweenEffects/Create New Fade Out Effect")]
    public class FadeOutEffect : TweenEffect<SpriteRenderer>
    {
        public override void ExecuteEffect(SpriteRenderer spriteRenderer, Action action = null)
        {
            if (!spriteRenderer) return;
            Color color = spriteRenderer.color;
            // Tween the alpha value of the color
            DOTween.To(
                () => color.a, alpha => { color.a = alpha; spriteRenderer.color = color; }, 0f, speed).SetEase(ease).OnComplete(() =>
            {
                action?.Invoke();
            });
        }
    }
}