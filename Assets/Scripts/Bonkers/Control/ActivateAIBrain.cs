using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bonkers.Combat;

namespace Bonkers.Control
{
    public class ActivateAIBrain : MonoBehaviour
    {
        TurbBodySensor bodySensorComponent;
        void Start()
        {
            bodySensorComponent = transform.parent.GetComponentInChildren<TurbBodySensor>();
        }

        //Animation Event
        void ActivateAI()
        {
            transform.parent.GetComponent<AIControl>().enabled = true;
            transform.parent.GetComponent<EnemyCombat>().enabled = true;
            bodySensorComponent.isEnabled = true;
        }
        
    }

}
