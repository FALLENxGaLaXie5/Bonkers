using System.Collections.Generic;
using UnityEngine;

namespace  Bonkers.Effects
{
    public class BlokEffects : MonoBehaviour
    {
        [SerializeField] List<TweenEffect> primaryImpactEffects;
        [SerializeField] List<TweenEffect> alternateImpactEffects;

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
    }
}