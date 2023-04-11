using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bonkers.SceneManagement
{

    public class Portal : MonoBehaviour
    {
        private static Portal instance;
        
        [SerializeField] float fadeOutTime = 3f;
        [SerializeField] float fadeInTime = 2f;
        [SerializeField] float waitTimeWhileFaded = 1f;

        int sceneToLoad = -1;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                DontDestroyOnLoad(transform.root);
                instance = this;
            }
        }

        public void StartTransition(int newSceneToLoad)
        {
            sceneToLoad = newSceneToLoad;
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
            sceneToLoad = i;
        }
    }
}