using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bonkers.Combat
{
    public class AnimateFragmentOut : MonoBehaviour
    {
        #region Class Variables
        MeshRenderer renderer;

        #endregion

        #region Class Functions
        public void Fade(float duration)
        {
            StartCoroutine(FadeOutAndDestroy(duration));
        }

        public void AssignRenderer()
        {
            renderer = GetComponent<MeshRenderer>();
        }

        IEnumerator FadeOutAndDestroy(float duration)
        {
            Color start = Color.white;
            Color end = new Color(1, 1, 1, 0);


            for (float t = 0f; t < duration; t += Time.deltaTime)
            {
                float normalizedTime = t / duration;
                renderer.material.color = Color.Lerp(start, end, normalizedTime);
                yield return null;
            }

            Destroy(this.gameObject);
        }
        #endregion
    }
}
