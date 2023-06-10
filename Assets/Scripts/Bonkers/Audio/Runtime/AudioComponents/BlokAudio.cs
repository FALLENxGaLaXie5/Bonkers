using Sirenix.OdinInspector;
using UnityEngine;

namespace Bonkers.Audio.Runtime
{
    [RequireComponent(typeof(AudioSource))]
    public class BlokAudio : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [InlineEditor][SerializeField] private AudioEvent bonkedAudioEvent;

        public void PlayBonkSound()
        {
            if (!bonkedAudioEvent)
            {
                Debug.LogWarning("Bonked audio event not set up!");
                return;
            }
            if (!audioSource)
            {
                Debug.LogWarning("Audio source not set up!");
                return;
            }
            bonkedAudioEvent.Play(audioSource);
        }

        public void PlayBreakSound()
        {
            
        }

        public void PlayImpactSound()
        {
            
        }
    }
}