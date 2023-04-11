using System.Collections.Generic;
using UnityEngine;

namespace  Bonkers.Events
{
    [CreateAssetMenu]
    public class LegacyGameEvent : ScriptableObject
    {
        /// <summary>
        /// The list of listeners that this event will notify if it is raised.
        /// </summary>
        private readonly List<LegacyGameEventListener> eventListeners = 
            new List<LegacyGameEventListener>();

        public virtual void Raise()
        {
            for (int i = eventListeners.Count - 1; i >= 0; i--)
                eventListeners[i].OnEventRaised();
        }

        public virtual void RegisterListener(LegacyGameEventListener listener)
        {
            if (!eventListeners.Contains(listener))
                eventListeners.Add(listener);
        }

        public virtual void UnregisterListener(LegacyGameEventListener listener)
        {
            if (eventListeners.Contains(listener))
                eventListeners.Remove(listener);
        }
    }

}
