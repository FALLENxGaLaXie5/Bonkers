using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Bonkers.Events;

namespace Bonkers.Drops
{
    public class ToxicPuddleBehavior : MonoBehaviour, IEnvironmentEffector
    {
        #region Inspector/Public Variables

        [SerializeField] PuddleDrop puddle = null;
        [SerializeField] float chanceOfPuddleSpawn = 20f;

        [SerializeField] UnityEvent<Vector3> AttemptSpawnEvent; 
        #endregion

        #region Class/Private Variables

        Animator animator;

        #endregion

        #region Unity Events 
        void Start() => animator = GetComponent<Animator>();

        #endregion

        #region Class functions

        IEnumerator WaitToDestroy()
        {
            yield return new WaitForSeconds(puddle.GetLife());
            //try to spawn new toxicccc life!
            AttemptSpawnNewToxicSlimo();
            animator.SetTrigger("destroy");
        }

        void AttemptSpawnNewToxicSlimo()
        {
            //if random generator does not favor spawning one, return
            if (UnityEngine.Random.Range(0, 100) >= chanceOfPuddleSpawn) return;

            AttemptSpawnEvent?.Invoke(transform.position);
        }

        //animation event
        void StartWaitingToDestroy()
        {
            StartCoroutine(WaitToDestroy());
        }

        //animation event
        void Destroy()
        {
            Destroy(gameObject);
        }
        
        public ScriptableObject AttemptGetEffector() => puddle;

        #endregion
    }
}

