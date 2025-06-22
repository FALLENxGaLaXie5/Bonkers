using Bonkers.ItemDrops;
using Bonkers.Movement;
using DG.Tweening;
using UnityEngine;

namespace Bonkers.Control
{
    [RequireComponent(typeof(PlayerEnvironmentEffectorGrabber))]
    public class PlayerEnvironmentEffectorsControl : MonoBehaviour
    {
        private PlayerEnvironmentEffectorGrabber _playerEnvironmentEffectorGrabber;
        private PlayerMovement _playerMovement;
        private SpriteRenderer _playerSpriteRenderer;

        private Tween _currentSpeedTween;
        private float _originalSpeed;
        private bool _isSpeedModified;

        #region Unity Events/Callbacks

        private void Awake()
        {
            _playerEnvironmentEffectorGrabber = GetComponent<PlayerEnvironmentEffectorGrabber>();
            _playerMovement = GetComponent<PlayerMovement>();
            _playerSpriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start() => _originalSpeed = _playerMovement.GetMoveSpeed();

        private void OnEnable()
        {
            _playerEnvironmentEffectorGrabber.OnEnterEnvironmentEffector += PlayerHitEnvironmentalEffector;
            _playerEnvironmentEffectorGrabber.OnExitEnvironmentEffector += PlayerExitEnvironmentalEffector;
        }

        private void OnDisable()
        {
            _playerEnvironmentEffectorGrabber.OnEnterEnvironmentEffector -= PlayerHitEnvironmentalEffector;
            _playerEnvironmentEffectorGrabber.OnExitEnvironmentEffector -= PlayerExitEnvironmentalEffector;
            // Clean up any active tweens
            _currentSpeedTween?.Kill();
        }

        #endregion

        private void PlayerHitEnvironmentalEffector(ScriptableObject effector)
        {
            PuddleDrop puddle = effector as PuddleDrop;
            if (!puddle) return;

            // Prevent multiple triggers while an effect is already active
            if (_isSpeedModified) return;

            // Kill any existing speed tween
            _currentSpeedTween?.Kill();

            float currentSpeed = _playerMovement.GetMoveSpeed();
            float targetSpeed = _originalSpeed - puddle.EffectStrength;

            // Ensure target speed doesn't go below a minimum threshold
            targetSpeed = Mathf.Max(targetSpeed, 0.5f);

            _currentSpeedTween = DOTween.To(
                () => currentSpeed,
                speed => _playerMovement.SetMoveSpeed(speed),
                targetSpeed,
                puddle.EffectTransitionDuration
            ).SetEase(Ease.OutQuad);

            // Apply visual effects to the player
            puddle.ApplyPlayerVisualEffects(_playerSpriteRenderer);

            _isSpeedModified = true;
        }

        private void PlayerExitEnvironmentalEffector(ScriptableObject effector)
        {
            PuddleDrop puddle = effector as PuddleDrop;
            if (!puddle) return;

            if (!_isSpeedModified) return;
            
            // Kill any existing speed tween
            _currentSpeedTween?.Kill();

            float currentSpeed = _playerMovement.GetMoveSpeed();

            _currentSpeedTween = DOTween.To(
                    () => currentSpeed,
                    speed => _playerMovement.SetMoveSpeed(speed),
                    _originalSpeed,
                    puddle.EffectTransitionDuration
                ).SetEase(Ease.OutQuad)
                .OnComplete(() => _isSpeedModified = false);

            // Stop visual effects on the player
            puddle.StopPlayerVisualEffects(_playerSpriteRenderer);
        }
    }
}