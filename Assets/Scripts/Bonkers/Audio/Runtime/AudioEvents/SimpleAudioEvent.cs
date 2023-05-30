using UnityEngine;
using Bonkers.Helpers.Runtime;
using Mono.CompilerServices.SymbolWriter;

namespace Bonkers.Audio.Runtime
{
    [CreateAssetMenu(fileName = "New Simple Audio Event", menuName = "Audio Events/Simple", order = 0)]
    public class SimpleAudioEvent : AudioEvent
    {
        [SerializeField] private AudioClip[] clips;
        [SerializeField] private RangedFloat volume;

        [MinMaxRange(0, 2)] public RangedFloat pitch;
        
        public override void Play(AudioSource source)
        {
            if (clips.Length == 0) return;

            source.clip = clips[Random.Range(0, clips.Length)];
            source.volume = Random.Range(volume.minValue, volume.maxValue);
            source.pitch = Random.Range(pitch.minValue, pitch.maxValue);
            source.Play();
        }
    }
}