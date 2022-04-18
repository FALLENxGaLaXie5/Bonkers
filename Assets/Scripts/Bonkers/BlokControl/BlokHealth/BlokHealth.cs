using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bonkers.Combat;

namespace Bonkers.BlokControl
{
    [RequireComponent(typeof(BlokDestroyIntoPoolHelper))]
    public abstract class BlokHealth : MonoBehaviour
    {
        protected ExplodeOnOrder explosionOrder;
        protected IBlokControl blokControl;
        protected BlokDestroyIntoPoolHelper blokDestroyIntoPoolHelper;

        public event Action<float> OnDestroyBlok;
        public event Action OnRespawnBlok;

        protected virtual void Awake()
        {
            explosionOrder = GetComponent<ExplodeOnOrder>();
            blokControl = GetComponent<IBlokControl>();
            blokDestroyIntoPoolHelper = GetComponent<BlokDestroyIntoPoolHelper>();
        }

        protected virtual void OnEnable() => OnDestroyBlok += blokDestroyIntoPoolHelper.AttemptSendBlokToPool;
        protected virtual void OnDisable() => OnDestroyBlok -= blokDestroyIntoPoolHelper.AttemptSendBlokToPool;

        public virtual void DestroyBlok()
        {
            OnDestroyBlok?.Invoke(explosionOrder.FadeTime);
            explosionOrder.ExplodeBlok();
        }

        public virtual void InvokeRespawnBlok()
        {
            OnRespawnBlok?.Invoke();
        }
    }
}
