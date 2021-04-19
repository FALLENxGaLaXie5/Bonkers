using Bonkers.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bonkers.BlokControl
{
    public class BlokHealth : MonoBehaviour
    {
        protected ExplodeOnOrder explosionOrder;
        protected IBlokControl blokControl;
        protected void Start()
        {
            explosionOrder = GetComponent<ExplodeOnOrder>();
            blokControl = GetComponent<IBlokControl>();
        }
        public virtual void DestroyBlok()
        {
            blokControl.PlaySound();
            explosionOrder.ExplodeBlok();
        }
    }

}
