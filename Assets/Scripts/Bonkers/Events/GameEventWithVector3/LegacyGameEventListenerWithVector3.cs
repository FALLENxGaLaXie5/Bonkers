using UnityEngine;
using UnityEngine.Events;

namespace  Bonkers.Events
{
    public class LegacyGameEventListenerWithVector3 : MonoBehaviour
    {
        [Tooltip("Event to register with.")]
        public LegacyGameEventWithVector3 Event;

        [Tooltip("Response to invoke when Event is raised.")]
        public UnityEvent<Vector3> Response;

        void OnEnable()
        {
            Event.RegisterListener(this);
        }

        void OnDisable()
        {
            Event.UnregisterListener(this);
        }

        public void OnEventRaised(Vector3 vector3)
        {
            Response.Invoke(vector3);
        }
    }
}