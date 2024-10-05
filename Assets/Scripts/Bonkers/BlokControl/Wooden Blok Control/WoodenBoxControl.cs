using UnityEngine;
using Bonkers.Combat;
using Bonkers.Effects;
using Bonkers.ItemDrops;

namespace Bonkers.BlokControl
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(IBlokInteraction))]
    public class WoodenBoxControl : MoveableBlokControl
    {
        AudioSource breakingSound;
        WoodenBlokBonks woodenBlokBonksComponent;
        BlokDroppable blokDroppableComponent;

        protected  override void Awake()
        {
            base.Awake();
            breakingSound = GetComponent<AudioSource>();
            woodenBlokBonksComponent = GetComponent<WoodenBlokBonks>();
            blokDroppableComponent = GetComponent<BlokDroppable>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            blokInteraction.OnBlokHit += WoodenBlokHit;
            blokInteraction.OnBlokHit += AttemptHitEffect;
            blokHealth.OnRespawnBlok += woodenBlokBonksComponent.ResetNumberTimesBonked;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            blokInteraction.OnBlokHit -= WoodenBlokHit;
            blokInteraction.OnBlokHit -= AttemptHitEffect;
            blokHealth.OnRespawnBlok -= woodenBlokBonksComponent.ResetNumberTimesBonked;
        }

        void WoodenBlokHit()
        {
            woodenBlokBonksComponent.IncrementNumTimesBonked();
            if (woodenBlokBonksComponent.IsBroken) blokHealth.BreakBlok();
        }
        
        void AttemptHitEffect()
        {
            if (woodenBlokBonksComponent.NumTimesBonked < woodenBlokBonksComponent.NumberBonksToBreak)
                blokEffects.ExecuteImpactEffects(transform, BlokEffects.TypeEffects.Primary);
        }
    }
}
