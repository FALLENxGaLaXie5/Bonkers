using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class MeshRendererExtension
{
    public static void Fade(this MeshRenderer renderer, MonoBehaviour mono, float duration, Action<MeshRenderer> callback = null)
    {
        mono.StartCoroutine(FadeCoroutine(renderer, duration, callback));
        //FadeCoroutine(renderer, duration, callback);
    }

    static IEnumerator FadeCoroutine(MeshRenderer renderer, float duration, Action<MeshRenderer> callback)
    {
        Color start = Color.white;
        Color end = new Color(1, 1, 1, 0);

        
        for (float t= 0f; t<duration; t+= Time.deltaTime)
        {
            float normalizedTime = t / duration;
            renderer.material.color = Color.Lerp(start, end, normalizedTime);
            yield return null;
        }
        
        

        Debug.Log("Sploded!");
        //Callback
        if (callback != null)
            callback(renderer);
    }

}
