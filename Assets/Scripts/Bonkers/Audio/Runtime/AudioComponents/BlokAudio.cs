using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Bonkers.Audio.Runtime
{
    [RequireComponent(typeof(AudioSource))]
    public class BlokAudio : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [InlineEditor][SerializeField] private AudioEvent _bonkedAudioEvent;
        [InlineEditor][SerializeField] private AudioEvent _blokImpactAudioEvent;
        [InlineEditor][SerializeField] private AudioEvent _blokDestroyedAudioEvent;
        [InlineEditor][SerializeField] private AudioEvent _blokSpawnAudioEvent;
        
        public void PlayBonkSound()
        {
            if (!_bonkedAudioEvent)
            {
                Debug.LogWarning("Bonked audio event not set up!");
                return;
            }
            if (!audioSource)
            {
                Debug.LogWarning("Audio source not set up!");
                return;
            }
            _bonkedAudioEvent.Play(audioSource);
        }

        public void PlayDestroySound()
        {
            if (!_blokDestroyedAudioEvent)
            {
                Debug.LogWarning("Blok destroyed audio event not set up!");
                return;
            }
            if (!audioSource)
            {
                Debug.LogWarning("Audio source not set up!");
                return;
            }
            _blokDestroyedAudioEvent.Play(audioSource);
        }
        
        public void PlaySpawnAudioEvent()
        {
            if (!_blokSpawnAudioEvent)
            {
                Debug.LogWarning("Blok spawned audio event not set up!");
                return;
            }
            if (!audioSource)
            {
                Debug.LogWarning("Audio source not set up!");
                return;
            }
            _blokSpawnAudioEvent.Play(audioSource);
        }

        public void PlayImpactSound()
        {
            if (!_blokImpactAudioEvent)
            {
                Debug.LogWarning("Blok impact audio event not set up!");
                return;
            }
            if (!audioSource)
            {
                Debug.LogWarning("Audio source not set up!");
                return;
            }
            _blokImpactAudioEvent.Play(audioSource);
        }
    }
}