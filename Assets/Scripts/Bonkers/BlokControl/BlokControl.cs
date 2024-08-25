using Bonkers.Audio.Runtime;
using Bonkers.Combat;
using UnityEngine;
using Bonkers.Effects;

namespace Bonkers.BlokControl
{
    [RequireComponent(typeof(BlokHealth))]
    [RequireComponent(typeof(BlokEffects))]
    [RequireComponent(typeof(BlokInteraction))]
    [RequireComponent(typeof(BlokAudio))]
    public class BlokControl : MonoBehaviour
    {
        protected BlokHealth blokHealth;
        protected BlokEffects blokEffects;
        protected BlokInteraction blokInteraction;

        private BlokAudio blokAudio;

        protected virtual void Awake()
        {
            blokHealth = GetComponent<BlokHealth>();
            blokEffects = GetComponent<BlokEffects>();
            blokInteraction = GetComponent<BlokInteraction>();
            blokAudio = GetComponent<BlokAudio>();
        }

        protected virtual void OnEnable()
        {
            blokInteraction.OnTriggerBonkAudio += blokAudio.PlayBonkSound;
            blokInteraction.OnBlokImpact += blokAudio.PlayImpactSound;
            blokHealth.OnRespawnBlok += RespawnBlokEffects;
        }

        protected virtual void OnDisable()
        {
            blokInteraction.OnTriggerBonkAudio -= blokAudio.PlayBonkSound;
            blokInteraction.OnBlokImpact -= blokAudio.PlayImpactSound;
            blokHealth.OnRespawnBlok -= RespawnBlokEffects;
        }


        protected virtual void DestroyBlok() => blokHealth.BreakBlok();
        protected virtual void RespawnBlokEffects() => blokAudio.PlaySpawnAudioEvent();
    }
}