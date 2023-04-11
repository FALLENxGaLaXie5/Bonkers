using UnityEngine;
using Bonkers.Effects;

namespace Bonkers.BlokControl
{
    [RequireComponent(typeof(BlokHealth))]
    [RequireComponent(typeof(BlokEffects))]
    [RequireComponent(typeof(AudioSource))]

    public class BlokControl : MonoBehaviour
    {
        protected BlokHealth health;
        protected BlokEffects blokEffects;
        protected AudioSource hitSound;

        protected virtual void Awake()
        {
            health = GetComponent<BlokHealth>();
            blokEffects = GetComponent<BlokEffects>();
            hitSound = GetComponentInChildren<AudioSource>();
        }

        protected virtual void OnEnable()
        {
            health.OnBreakBlok += PlayBreakSound;
        }

        protected virtual void OnDisable()
        {
            health.OnBreakBlok -= PlayBreakSound;
        }
        
        /// <summary>
        /// Play generic blok hitting sound
        /// </summary>
        /// <param name="waitTime"></param>
        protected virtual void PlaySound()
        {
            if (!hitSound.isActiveAndEnabled) return;
            hitSound.Play();
        }
        
        /// <summary>
        /// Play sound for destroying blok (can use wait time specified, but likely just want to play immediately)
        /// </summary>
        /// <param name="waitTime"></param>
        protected virtual void PlayBreakSound(float waitTime)
        {
            if (!hitSound.isActiveAndEnabled) return;
            hitSound.Play();
        }

        protected virtual void DestroyBlok()
        {
            health.BreakBlok();
        }
    }
}