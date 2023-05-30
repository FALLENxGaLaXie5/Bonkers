using Sirenix.OdinInspector;
using UnityEngine;

namespace Bonkers.Audio.Runtime
{
    [RequireComponent(typeof(AudioSource))]
    public class BlokAudio : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [InlineEditor][SerializeField] private AudioEvent bonkedAudioEvent;

        public void PlayBonkSound() => bonkedAudioEvent.Play(audioSource);

        public void PlayBreakSound()
        {
            
        }

        public void PlayImpactSound()
        {
            
        }
    }
}