using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bonkers.Combat;

namespace Bonkers.BlokControl
{
    public abstract class BlokHealth : MonoBehaviour
    {
        protected ExplodeOnOrder explosionOrder;
        protected IBlokControl blokControl;
        public event Action onDestroyBlok;

        protected void Start()
        {
            explosionOrder = GetComponent<ExplodeOnOrder>();
            blokControl = GetComponent<IBlokControl>();
        }
        public virtual void DestroyBlok()
        {
            onDestroyBlok?.Invoke();
            explosionOrder.ExplodeBlok();
        }
    }

}
