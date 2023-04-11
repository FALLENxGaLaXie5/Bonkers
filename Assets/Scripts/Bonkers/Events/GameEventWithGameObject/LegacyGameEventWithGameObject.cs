using System.Collections.Generic;
using UnityEngine;

namespace  Bonkers.Events
{
    [CreateAssetMenu]
    public class LegacyGameEventWithGameObject : ScriptableObject
    {
        /// <summary>
        /// The list of listeners that this event will notify if it is raised.
        /// </summary>
        private readonly List<LegacyGameEventListenerWithGameObject> eventListeners = 
            new List<LegacyGameEventListenerWithGameObject>();

        public void Raise(GameObject obj)
        {
            for (int i = eventListeners.Count - 1; i >= 0; i--)
                eventListeners[i].OnEventRaised(obj);
        }

        public void RegisterListener(LegacyGameEventListenerWithGameObject listener)
        {
            if (!eventListeners.Contains(listener))
                eventListeners.Add(listener);
        }

        public void UnregisterListener(LegacyGameEventListenerWithGameObject listener)
        {
            if (eventListeners.Contains(listener))
                eventListeners.Remove(listener);
        }
    }

}