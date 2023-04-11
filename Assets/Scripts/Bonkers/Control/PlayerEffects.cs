using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bonkers.Control
{
    public class PlayerEffects : MonoBehaviour
    {
        [SerializeField] ParticleSystem[] boostEffects;
        // Start is called before the first frame update
        
        public void PlayBoostEffect()
        {
            for (int i = 0; i < boostEffects.Length; i++)
            {
                boostEffects[i].Play();
            }
        }

        public void StopBoostEffect()
        {
            for (int i = 0; i < boostEffects.Length; i++)
            {
                boostEffects[i].Stop();
            }
        }
    }
}