using UnityEngine;
using UnityEngine.Events;

namespace  Bonkers.Events
{
    public class GameEventListenerWithGameObject : MonoBehaviour
    {
        [Tooltip("Event to register with.")]
        public GameEventWithGameObject Event;

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