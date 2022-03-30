using System.Collections.Generic;
using UnityEngine;

namespace  Bonkers.Events
{
    [CreateAssetMenu]
    public class GameEventWithVector3 : ScriptableObject
    {
        /// <summary>
        /// The list of listeners that this event will notify if it is raised.
        /// </summary>
        private readonly List<GameEventListenerWithVector3> eventListeners = 
            new List<GameEventListenerWithVector3>();

        public void Raise(Vector3 vector3)
        {
            for (int i = eventListeners.Count - 1; i >= 0; i--)
                eventListeners[i].OnEventRaised(vector3);
        }

        public void RegisterListener(GameEventListenerWithVector3 listener)
        {
            if (!eventListeners.Contains(listener))
                eventListeners.Add(listener);
        }

        public void UnregisterListener(GameEventListenerWithVector3 listener)
        {
            if (eventListeners.Contains(listener))
                eventListeners.Remove(listener);
        }
    }

}