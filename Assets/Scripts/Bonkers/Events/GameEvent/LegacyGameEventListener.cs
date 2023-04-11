using UnityEngine;
using UnityEngine.Events;

namespace  Bonkers.Events
{
    public class LegacyGameEventListener : MonoBehaviour
    {
        [Tooltip("Event to register with.")]
        public LegacyGameEvent Event;

        [Tooltip("Response to invoke when Event is raised.")]
        public UnityEvent Response;

        void OnEnable()
        {
            Event.RegisterListener(this);
        }

        void OnDisable()
        {
            Event.UnregisterListener(this);
        }

        public void OnEventRaised()
        {
            Response.Invoke();
        }
    }
}