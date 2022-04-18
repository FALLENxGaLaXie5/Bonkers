using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// To be used as the base class to inherit from for all Blok Interaction Scripts (BasicBlokInteraction, Glass Blok Interaction, WoodenBlokInteraction...)
/// Should contain all common functionality - essentially these fields
/// </summary>
namespace Bonkers.Combat
{
    public abstract class BlokInteraction : MonoBehaviour
    {
        [SerializeField] protected float checkRadius = 0.1f;
        // TODO: Figure out a way to set default bonkableLayers in code so it does not need to be set in inspector on different prefabs
        [SerializeField] protected LayerMask bonkableLayers;

        public Action<bool, Vector3> OnSetMoving;
        public Action OnBlokHit;
        public Action<Vector3, Vector3> OnBlokBumped;
        public event Action OnBlokImpact;
        public event Action OnBlokDestroyInImpact;

        public void TriggerBlokImpact()
        {
            OnBlokImpact?.Invoke();
        }

        protected virtual void InvokeOnBlokDestroyInImpact()
        {
            OnBlokDestroyInImpact?.Invoke();
        }
    }
}