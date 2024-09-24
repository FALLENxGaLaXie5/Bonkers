using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bonkers.Combat;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace Bonkers.Control
{
    public class ActivateDumbAIBrain : MonoBehaviour
    {
        private bool _dead;
        private TurbBodySensor _bodySensorComponent;
        private SpriteRenderer _spriteRenderer;
        //private Animator brainActivationAnimator;

        public void DisableControlActivation(bool dead = true) => _dead = dead;

        private void Awake()
        {
            //brainActivationAnimator = GetComponent<Animator>();
            _bodySensorComponent = GetComponentInChildren<TurbBodySensor>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            // Ensure the sprite starts invisible
            Color color = _spriteRenderer.color;
            color.a = 0;
            _spriteRenderer.color = color;

            // Start the fade-in sequence
            StartFadeIn();
        }

        /// <summary>
        /// Fade the alpha from 0 to 1 over 2 seconds
        /// </summary>
        private void StartFadeIn() => _spriteRenderer.DOFade(1f, 2f).OnComplete(ActivateAI);

        void ActivateAI()
         {
            if (_dead) return;
            GetComponent<AIControl>().enabled = true;
            GetComponent<EnemyCombat>().enabled = true;
            _bodySensorComponent.isEnabled = true;
            //brainActivationAnimator.enabled = false;
        }
    }
}
