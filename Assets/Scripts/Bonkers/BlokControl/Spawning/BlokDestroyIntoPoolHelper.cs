using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Bonkers.BlokControl
{
    public class BlokDestroyIntoPoolHelper : MonoBehaviour
    {
        #region Inspector/Public Variables
        [SerializeField] private IndividualBlokPoolingData poolingData;
        [SerializeField] private UnityEvent<GameObject> AttemptSendBlokToPoolEvent;
        
        public IndividualBlokPoolingData PoolingData => poolingData;
        
        #endregion

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
            AttemptSendBlokToPoolEvent?.Invoke(gameObject);
            gameObject.SetActive(false);
        }
    }
}