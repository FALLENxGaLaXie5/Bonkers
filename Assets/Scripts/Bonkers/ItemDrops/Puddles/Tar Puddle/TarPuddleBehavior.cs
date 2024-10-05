using System.Collections;
using UnityEngine;

namespace Bonkers.ItemDrops
{
    public class TarPuddleBehavior : MonoBehaviour, IEnvironmentEffector
    {
        #region Inspector/Public Variables

        [SerializeField] PuddleDrop puddle = null;

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
            
            animator.SetTrigger("destroy");
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