using UnityEngine;
using UnityEngine.Events;

namespace Bonkers.Events
{
    public abstract class BaseGameEventListener<T, GE, UER> : MonoBehaviour
        where GE : BaseGameEvent<T>
        where UER : UnityEvent<T>
    {
        [SerializeField]
        protected GE _GameEvent;

        [SerializeField]
        protected UER _UnityEventResponse;

        protected void OnEnable()
        {
            if (_GameEvent is null) return;
            _GameEvent.EventListeners += TriggerResponses; // Subscribe
        }

        protected void OnDisable()
        {
            if (_GameEvent is null) return;
            _GameEvent.EventListeners -= TriggerResponses; // Unsubscribe
        }

        [ContextMenu("Trigger Responses")]
        public void TriggerResponses(T val)
        {
            //No need to nullcheck here, UnityEvents do that for us (lets avoid the double nullcheck)
            _UnityEventResponse.Invoke(val);
        }
    }
}