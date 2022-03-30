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

        public event Action onDestroyBlok;

        protected virtual void Awake()
        {
            explosionOrder = GetComponent<ExplodeOnOrder>();
            blokControl = GetComponent<IBlokControl>();
            blokDestroyIntoPoolHelper = GetComponent<BlokDestroyIntoPoolHelper>();
        }

        protected virtual void OnEnable() => onDestroyBlok += blokDestroyIntoPoolHelper.AttemptSendBlokToPool;
        protected virtual void OnDisable() => onDestroyBlok -= blokDestroyIntoPoolHelper.AttemptSendBlokToPool;

        public virtual void DestroyBlok()
        {
            onDestroyBlok?.Invoke();
            explosionOrder.ExplodeBlok();
        }
    }
}
