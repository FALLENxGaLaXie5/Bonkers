using System;
using DG.Tweening;
using UnityEngine;

namespace Bonkers.Effects
{
    [CreateAssetMenu(fileName = "New Color Lerp Effect", menuName = "TweenEffects/Create New Color Lerp Effect")]
    public class ColorLerpEffect : TweenEffect<SpriteRenderer>
    {
        [SerializeField] private Color targetColor = Color.red;
        [SerializeField] private bool rememberOriginalColor = true;
        
        private Color _originalColor;
        private Tween _currentTween;

        public override void ExecuteEffect(SpriteRenderer spriteRenderer, Action action = null)
        {
            if (!spriteRenderer) return;

            // Kill any existing tween
            _currentTween?.Kill();

            // Remember the original color if needed
            if (rememberOriginalColor)
            {
                _originalColor = spriteRenderer.color;
            }

            // Create pulsing effect using DOTween's built-in looping
            _currentTween = DOTween.To(
                () => spriteRenderer.color,
                color => spriteRenderer.color = color,
                targetColor,
                speed)
                .SetEase(ease)
                .SetLoops(-1, LoopType.Yoyo) // Infinite loops with yoyo (back and forth)
                .SetLink(spriteRenderer.gameObject);
                
            action?.Invoke();
        }

        public override void StopEffect(SpriteRenderer spriteRenderer, Action action = null)
        {
            if (!spriteRenderer) return;

            // Kill any existing tween
            _currentTween?.Kill();

            // Tween back to the original color from current state
            _currentTween = DOTween.To(
                () => spriteRenderer.color,
                color => spriteRenderer.color = color,
                _originalColor,
                speed)
                .SetEase(ease)
                .SetLink(spriteRenderer.gameObject)
                .OnComplete(() =>
                {
                    action?.Invoke();
                });
        }
    }
}