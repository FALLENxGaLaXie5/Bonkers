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
        protected BreakOnOrder explosionOrder;
        protected IMoveableBlokControl blokControl;
        protected BlokDestroyIntoPoolHelper blokDestroyIntoPoolHelper;

        public event Action<float> OnBreakBlok;
        public event Action OnRespawnBlok;

        protected virtual void Awake()
        {
            explosionOrder = GetComponent<BreakOnOrder>();
            blokControl = GetComponent<IMoveableBlokControl>();
            blokDestroyIntoPoolHelper = GetComponent<BlokDestroyIntoPoolHelper>();
        }

        protected virtual void OnEnable()
        {
            OnBreakBlok += blokDestroyIntoPoolHelper.AttemptSendBlokToPool;
            blokDestroyIntoPoolHelper.OnFailToPool += DestroyBlok;
        }

        protected virtual void OnDisable()
        {
            OnBreakBlok -= blokDestroyIntoPoolHelper.AttemptSendBlokToPool;
            blokDestroyIntoPoolHelper.OnFailToPool -= DestroyBlok;
        }

        /// <summary>
        /// Breaks bloks into it's pieces (will also attempt to pool it; if no pooling event set, will destroy completely)
        /// </summary>
        public virtual void BreakBlok()
        {
            OnBreakBlok?.Invoke(explosionOrder.FadeTime);
            explosionOrder.BreakBlok();
        }

        /// <summary>
        /// This will actually completely destroy the blok and it's fragments
        /// </summary>
        /// <param name="waitTime"></param>
        /// <param name="fragmentComponents"></param>
        public virtual void DestroyBlok(float waitTime, List<AnimateFragmentOut> fragmentComponents)
        {
            foreach (var fragment in fragmentComponents)
            {
                Destroy(fragment.gameObject, waitTime);
            }
            Destroy(gameObject, waitTime);
        }

        public virtual void InvokeRespawnBlok()
        {
            OnRespawnBlok?.Invoke();
        }
    }
}
