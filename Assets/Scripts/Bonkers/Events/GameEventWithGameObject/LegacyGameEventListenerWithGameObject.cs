using UnityEngine;
using UnityEngine.Events;

namespace  Bonkers.Events
{
    public class LegacyGameEventListenerWithGameObject : MonoBehaviour
    {
        [Tooltip("Event to register with.")]
        public LegacyGameEventWithGameObject Event;

        [Tooltip("Response to invoke when Event is raised.")]
        public UnityEvent<GameObject> Response;

        void OnEnable()
        {
            Event.RegisterListener(this);
        }

        void OnDisable()
        {
            Event.UnregisterListener(this);
        }

        public void OnEventRaised(GameObject obj)
        {
            Response.Invoke(obj);
        }
    }
}