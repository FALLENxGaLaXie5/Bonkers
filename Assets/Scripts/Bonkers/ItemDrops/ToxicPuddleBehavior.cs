using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bonkers.Movement;
using Bonkers.Core;
using System;

namespace Bonkers.Drops
{
    public class ToxicPuddleBehavior : MonoBehaviour
    {
        #region Inspector/Public Variables

        [SerializeField] GameObject toxicSlimoPrefab;
        [SerializeField] PuddleDrop puddle = null;
        [SerializeField] float chanceOfPuddleSpawn = 20f;

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
            //try to spawn new toxicccc life!
            AttemptSpawnNewToxicSlimo();
            animator.SetTrigger("destroy");
        }

        void AttemptSpawnNewToxicSlimo()
        {
            //if random generator does not favor spawning one, return
            if (UnityEngine.Random.Range(0, 100) >= chanceOfPuddleSpawn) return;

            Transform toxicAllianceTransform = FindObjectOfType<ToxicSlimoHolder>().transform;
            //only spawn one if we are not at max toxic slimos yet
            if (toxicAllianceTransform.childCount >= FindObjectOfType<SpawnSystem>().GetMaxToxicSlimos() + 5) return;

            GameObject newToxicSlimo = Instantiate(toxicSlimoPrefab, transform.position, Quaternion.identity);
            newToxicSlimo.transform.parent = toxicAllianceTransform;
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

