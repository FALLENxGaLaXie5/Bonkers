using UnityEngine;

namespace Bonkers.Audio.Runtime
{
    public abstract class AudioEvent : ScriptableObject
    {
        public abstract void Play(AudioSource source);
    }
}