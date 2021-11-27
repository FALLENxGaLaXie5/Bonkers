using UnityEngine;
using Bonkers.Combat;
using Bonkers.Drops;
using Bonkers.Effects;

namespace Bonkers.BlokControl
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(IBlokInteraction))]
    public class WoodenBoxControl : BlokControl
    {
        AudioSource breakingSound;
        WoodenBlokBonks bonksInstance;
        BlokDroppable blokDroppable;

        protected  override void Awake()
        {
            base.Awake();
            breakingSound = GetComponent<AudioSource>();
            bonksInstance = GetComponent<WoodenBlokBonks>();
            blokDroppable = GetComponent<BlokDroppable>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            blokInteraction.onBlokHit += WoodenBlokHit;
            blokInteraction.onBlokHit += AttemptHitEffect;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            blokInteraction.onBlokHit -= WoodenBlokHit;
            blokInteraction.onBlokHit -= AttemptHitEffect;
        }

        protected override void PlaySound()
        {
            breakingSound.Play();
        }

        void WoodenBlokHit()
        {
            bonksInstance.IncrementNumTimesBonked();
            if (bonksInstance.NumTimesBonked >= bonksInstance.NumberBonksToBreak)
            {
                blokDroppable.SpawnDrop();
                health.DestroyBlok();
            }
        }
        
        void AttemptHitEffect()
        {
            if (bonksInstance.NumTimesBonked < bonksInstance.NumberBonksToBreak)
                blokEffects.ExecuteImpactEffects(transform, BlokEffects.TypeEffects.Primary);
        }
    }
}
