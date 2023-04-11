using System.Collections.Generic;
using UnityEngine;

namespace  Bonkers.Events
{
    [CreateAssetMenu]
    public class LegacyGameEventWithVector3 : ScriptableObject
    {
        /// <summary>
        /// The list of listeners that this event will notify if it is raised.
        /// </summary>
        private readonly List<LegacyGameEventListenerWithVector3> eventListeners = 
            new List<LegacyGameEventListenerWithVector3>();

        public void Raise(Vector3 vector3)
        {
            for (int i = eventListeners.Count - 1; i >= 0; i--)
                eventListeners[i].OnEventRaised(vector3);
        }

        public void RegisterListener(LegacyGameEventListenerWithVector3 listener)
        {
            if (!eventListeners.Contains(listener))
                eventListeners.Add(listener);
        }

        public void UnregisterListener(LegacyGameEventListenerWithVector3 listener)
        {
            if (eventListeners.Contains(listener))
                eventListeners.Remove(listener);
        }
    }

}