using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RoboRyanTron.Unite2017.Variables;
using UnityEngine;
using UnityEngine.Events;

namespace Bonkers.BlokControl
{
    public class BlokDestroyIntoPoolHelper : MonoBehaviour
    {
        #region Inspector/Public Variables
        [SerializeField] private IndividualBlokPoolingData poolingData;
        //Just leave event blank if we don't want to pool this blok
        [SerializeField] private UnityEvent<GameObject> AttemptSendBlokToPoolEvent;
        [SerializeField] private FloatReference fragmentsFadeTime = new (5f);
        public IndividualBlokPoolingData PoolingData => poolingData;
        
        //Pass in a wait time that can be used along with the list of fragments to event
        public event Action<float, List<AnimateFragmentOut>> OnFailToPool;
        
        #endregion

        private List<AnimateFragmentOut> fragmentComponents = new List<AnimateFragmentOut>();

        private void Awake() => fragmentComponents = GetComponentsInChildren<AnimateFragmentOut>().ToList();

        /// <summary>
        /// Attempts to send blok to it's blok pool after a specified waiting period
        /// </summary>
        /// <param name="waitTime"></param>
        public void AttemptSendBlokToPool(float waitTime)
        {
            StartCoroutine(WaitTriggerBlokPooling(waitTime));
        }

        IEnumerator WaitTriggerBlokPooling(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            if (AttemptSendBlokToPoolEvent.GetPersistentEventCount() < 1)
            {
                OnFailToPool?.Invoke(fragmentsFadeTime, fragmentComponents);
            }
            else
            {
                AttemptSendBlokToPoolEvent?.Invoke(gameObject);
                gameObject.SetActive(false);
            }
        }
    }
}