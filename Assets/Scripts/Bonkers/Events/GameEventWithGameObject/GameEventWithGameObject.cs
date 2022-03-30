using System.Collections.Generic;
using UnityEngine;

namespace  Bonkers.Events
{
    [CreateAssetMenu]
    public class GameEventWithGameObject : ScriptableObject
    {
        /// <summary>
        /// The list of listeners that this event will notify if it is raised.
        /// </summary>
        private readonly List<GameEventListenerWithGameObject> eventListeners = 
            new List<GameEventListenerWithGameObject>();

        public void Raise(GameObject obj)
        {
            for (int i = eventListeners.Count - 1; i >= 0; i--)
                eventListeners[i].OnEventRaised(obj);
        }

        public void RegisterListener(GameEventListenerWithGameObject listener)
        {
            if (!eventListeners.Contains(listener))
                eventListeners.Add(listener);
        }

        public void UnregisterListener(GameEventListenerWithGameObject listener)
        {
            if (eventListeners.Contains(listener))
                eventListeners.Remove(listener);
        }
    }

}