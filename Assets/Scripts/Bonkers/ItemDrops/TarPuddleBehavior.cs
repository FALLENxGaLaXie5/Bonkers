using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bonkers.Movement;

namespace Bonkers.Drops
{
    public class TarPuddleBehavior : MonoBehaviour
    {
        #region Inspector/Public Variables

        [SerializeField] PuddleDrop puddle = null;

        #endregion

        #region Class/Private Variables

        Animator animator;

        #endregion

        #region Unity Events 
        void Start()
        {
            animator = GetComponent<Animator>();
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.transform.tag == "Player")
            {
                PlayerMovement playerMovement = collision.transform.GetComponent<PlayerMovement>();
                playerMovement.SetMoveSpeed(playerMovement.GetMoveSpeed() - puddle.GetEffect());
            }
        }

        void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.transform.tag == "Player")
            {
                PlayerMovement playerMovement = collision.transform.GetComponent<PlayerMovement>();
                playerMovement.ResetMoveSpeed();
            }
        }

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

        #endregion
    }
}

