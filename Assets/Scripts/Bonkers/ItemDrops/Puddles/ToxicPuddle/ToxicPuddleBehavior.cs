using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Bonkers.ItemDrops
{
    public class ToxicPuddleBehavior : MonoBehaviour, IEnvironmentEffector
    {
        #region Inspector/Public Variables

        [InlineEditor]
        [SerializeField] PuddleDrop puddle;
        [SerializeField] float chanceOfPuddleSpawn = 20f;

        [SerializeField] private UnityEvent<Vector3> AttemptSpawnEvent;
        #endregion

        #region Class/Private Variables

        private Animator _animator;

        #endregion

        #region Unity Events

        private void Awake() => _animator = GetComponent<Animator>();

        private void Start()
        {

        }

        #endregion

        #region Class functions

        IEnumerator WaitToDestroy()
        {
            yield return new WaitForSeconds(puddle.GetLife());
            //try to spawn new toxicccc life!
            AttemptSpawnNewToxicSlimo();
            //_animator.SetTrigger("destroy");
        }

        void AttemptSpawnNewToxicSlimo()
        {
            //if random generator does not favor spawning one, return
            if (UnityEngine.Random.Range(0, 100) >= chanceOfPuddleSpawn) return;

            AttemptSpawnEvent?.Invoke(transform.position);
        }

        //animation event
        void StartWaitingToDestroy() => StartCoroutine(WaitToDestroy());

        //animation event
        void Destroy() => Destroy(gameObject);

        public ScriptableObject AttemptGetEffector() => puddle;

        #endregion
    }
}

