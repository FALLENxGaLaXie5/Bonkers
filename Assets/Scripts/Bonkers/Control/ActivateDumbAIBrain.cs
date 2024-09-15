using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bonkers.Combat;

namespace Bonkers.Control
{
    public class ActivateDumbAIBrain : MonoBehaviour
    {
        public bool Dead { get; private set; }
        private TurbBodySensor bodySensorComponent;
        private Animator brainActivationAnimator;

        public void DisableControlActivation(bool dead = true) => Dead = dead;

        private void Awake()
        {
            brainActivationAnimator = GetComponent<Animator>();
            bodySensorComponent = transform.parent.GetComponentInChildren<TurbBodySensor>();
        }

        //Animation Event
        void ActivateAI()
         {
            if (Dead) return;
            transform.parent.GetComponent<AIControl>().enabled = true;
            transform.parent.GetComponent<EnemyCombat>().enabled = true;
            bodySensorComponent.isEnabled = true;
            brainActivationAnimator.enabled = false;
        }
    }

}
