using System.Collections;
using System.Collections.Generic;
using Bonkers._2DDestruction;
using UnityEngine;

public class AnimateFragmentOut : MonoBehaviour
{
    #region Class Variables
    MeshRenderer rend;

    #endregion

    #region Class Functions
    public void Fade(float duration)
    {
        StartCoroutine(FadeOutAndDestroy(duration));
    }

    public void AssignRenderer()
    {
        rend = GetComponent<MeshRenderer>();
    }

    IEnumerator FadeOutAndDestroy(float duration)
    {
        Color start = Color.white;
        Color end = new Color(1, 1, 1, 0);


        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            float normalizedTime = t / duration;
            rend.material.color = Color.Lerp(start, end, normalizedTime);
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
