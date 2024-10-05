using System;
using System.Collections.Generic;
using Bonkers.Audio.Runtime;
using UnityEngine;

namespace  Bonkers.Effects
{
    [RequireComponent(typeof(BlokAudio))]
    public class BlokEffects : MonoBehaviour
    {
        [SerializeField] List<TweenEffect> primaryImpactEffects;
        [SerializeField] List<TweenEffect> alternateImpactEffects;

        private BlokAudio _blokAudio;

        private void Awake() => _blokAudio = GetComponent<BlokAudio>();
        
        public enum TypeEffects
        {
            Primary,
            Alternate
        }
        
        public virtual void ExecuteImpactEffects(Transform transform, TypeEffects type)
        {
            if (type == TypeEffects.Primary)
                Execute(primaryImpactEffects);
            else if (type == TypeEffects.Alternate)
                Execute(alternateImpactEffects);
        }

        void Execute(List<TweenEffect> effects)
        {
            if (effects.Count <= 0) return;
            
            foreach (TweenEffect effect in effects)
            {
                effect.ExecuteEffect(transform);
            }
        }

        public void PlayBonkSound() => _blokAudio.PlayBonkSound();
        public void PlayRespawnSoundEffects() => _blokAudio.PlaySpawnAudioEvent();
        public void PlayImpactBlokSoundEffects(bool destroyInImpact)
        {
            if (destroyInImpact)
                _blokAudio.PlayDestroySound();
            else
                _blokAudio.PlayImpactSound();
        }
    }
}