using Bonkers.Combat;
using UnityEngine;
using Bonkers.Effects;

namespace Bonkers.BlokControl
{
    [RequireComponent(typeof(BlokHealth))]
    [RequireComponent(typeof(BlokEffects))]
    [RequireComponent(typeof(BlokInteraction))]
    public class BlokControl : MonoBehaviour
    {
        protected BlokHealth blokHealth;
        protected BlokEffects blokEffects;
        protected BlokInteraction blokInteraction;

        protected virtual void Awake()
        {
            blokHealth = GetComponent<BlokHealth>();
            blokEffects = GetComponent<BlokEffects>();
            blokInteraction = GetComponent<BlokInteraction>();
        }

        protected virtual void OnEnable()
        {
            blokInteraction.OnTriggerBonkAudio += blokEffects.PlayBonkSound;
            blokInteraction.OnBlokImpact += blokEffects.PlayImpactBlokSoundEffects;
            blokHealth.OnRespawnBlok += blokEffects.PlayRespawnSoundEffects;
        }

        protected virtual void OnDisable()
        {
            blokInteraction.OnTriggerBonkAudio -= blokEffects.PlayBonkSound;
            blokInteraction.OnBlokImpact -= blokEffects.PlayImpactBlokSoundEffects;
            blokHealth.OnRespawnBlok -= blokEffects.PlayRespawnSoundEffects;
        }


        protected virtual void DestroyBlok() => blokHealth.BreakBlok();
        
    }
}