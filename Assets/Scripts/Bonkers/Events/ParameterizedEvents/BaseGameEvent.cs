using System;
using UnityEngine;

namespace Bonkers.Events
{
    [Serializable]
    public abstract class BaseGameEvent<T> : ScriptableObject
    {
        // Event keyword makes it so that only this class can trigger the event
        //Public because anyone can subscribe(+=), and unsubscribe(-=) to/from this event
        public event Action<T> EventListeners = delegate {};

        public void Raise(T item)
        {
            EventListeners(item);
        }
    }
}