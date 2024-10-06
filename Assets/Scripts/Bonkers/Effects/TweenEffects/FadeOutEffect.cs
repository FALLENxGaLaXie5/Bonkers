using System;
using DG.Tweening;
using UnityEngine;

namespace Bonkers.Effects
{
    [CreateAssetMenu(fileName = "New Fade Out Effect", menuName = "TweenEffects/Create New Fade Out Effect")]
    public class FadeOutEffect : TweenEffect<SpriteRenderer>
    {
        public override void ExecuteEffect(SpriteRenderer spriteRenderer, Action action)
        {
            if (!spriteRenderer) return;

            // Use the existing color of the spriteRenderer directly
            Color color = spriteRenderer.color;
            color.a = 1;
            spriteRenderer.color = color;
            // Tween the alpha value of the color
            DOTween.To(
                () => color.a, // Getter function for the current alpha
                alpha => {
                    color.a = alpha; // Set the alpha value
                    spriteRenderer.color = color; // Update the sprite renderer's color
                },
                0f, // Target alpha
                speed // Duration
            ).SetEase(ease).OnComplete(() =>
            {
                action?.Invoke(); // This will be called after the tween completes
            });
        }
    }
}