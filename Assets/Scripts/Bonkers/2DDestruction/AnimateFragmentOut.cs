using System.Collections;
using System.Collections.Generic;
using Bonkers._2DDestruction;
using DG.Tweening;
using UnityEngine;

public class AnimateFragmentOut : MonoBehaviour
{
    #region Class Variables
    Renderer rend;

    #endregion

    #region Class Functions
    public void Fade(float duration)
    {
        //rend.material.DOFade(0f,duration);
        StartCoroutine(FadeOutAndDestroy(duration));
    }

    public void AssignRenderer()
    {
        rend = GetComponent<Renderer>();
    }

    IEnumerator FadeOutAndDestroy(float duration)
    {
        float start = 1f;
        float end = 0f;

        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            float normalizedTime = t / duration;
            float lerpedFloat = Mathf.Lerp(start, end, normalizedTime);
            rend.material.SetFloat("_Alpha", lerpedFloat);
            yield return null;
        }

        //Reset the fragment under it's parent when completely faded
        FragmentDataStorage fragmentDataStorage = GetComponent<FragmentDataStorage>();
        if (!fragmentDataStorage)
        {
            Debug.LogError("A fragment data storage didn't get added to this fragment! Wtf!");
            yield return null;
        }
        transform.parent = fragmentDataStorage.parent;
        transform.localPosition = fragmentDataStorage.localPosition;
        transform.gameObject.SetActive(false);
        
        //TODO: Take out comment if this works
        //Destroy(gameObject);
    }
    #endregion
}
