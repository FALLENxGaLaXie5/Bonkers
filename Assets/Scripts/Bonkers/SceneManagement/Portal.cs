using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Pathfinding;

namespace Bonkers.SceneManagement
{

    public class Portal : MonoBehaviour
    {

        [SerializeField] float fadeOutTime = 3f;
        [SerializeField] float fadeInTime = 2f;
        [SerializeField] float waitTimeWhileFaded = 1f;

        int sceneToLoad = -1;

        public void StartTransition(int newSceneToLoad)
        {
            this.sceneToLoad = newSceneToLoad;
            StartCoroutine(Transition());
        }

        IEnumerator Transition()
        {
            if (sceneToLoad < 0)
            {
                Debug.LogError("Scene to load not set.");
                yield break;
            }

            Fader fader = GetComponentInChildren<Fader>();

            yield return fader.FadeOut(fadeOutTime);

            yield return SceneManager.LoadSceneAsync(sceneToLoad);

            yield return new WaitForSeconds(waitTimeWhileFaded);

            yield return fader.FadeIn(fadeInTime);
        }

        public void SetSceneToLoad(int i)
        {
            this.sceneToLoad = i;
        }
    }
}