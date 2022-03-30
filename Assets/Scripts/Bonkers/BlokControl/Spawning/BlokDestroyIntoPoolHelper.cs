using UnityEngine;
using UnityEngine.Events;

namespace Bonkers.BlokControl
{
    public class BlokDestroyIntoPoolHelper : MonoBehaviour
    {
        #region Inspector/Public Variables
        [SerializeField] IndividualBlokPoolingData poolingData;
        [SerializeField] private UnityEvent<GameObject> AttemptSendBlokToPoolEvent;
        
        public IndividualBlokPoolingData PoolingData => poolingData;
        
        #endregion

        public void AttemptSendBlokToPool() => AttemptSendBlokToPoolEvent?.Invoke(gameObject);
    }
}