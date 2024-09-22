using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bonkers.Drops;
using Bonkers.Movement;

namespace Bonkers.Control
{
    public class ToxicSlimoControl : AISingleSpaceMovementControl
    {
        [SerializeField] PuddleDrop toxicPuddleDrop;
        [SerializeField] [Range(1f, 15f)] float minPuddleDropTime = 3f;
        [SerializeField] [Range(3f, 30f)] float maxPuddleDropTime = 7f;
        [SerializeField] [Range(0.01f, 0.3f)] float maxDistancePuddleSpawn = 0.02f;

        protected override void Awake()
        {
            base.Awake();
        }

        //Need to override base AIControl start functionality so it doesn't use the backtrack target function
        protected override void Start()
        {
            StartCoroutine(DropPuddles());
        }
        
        IEnumerator DropPuddles()
        {
            while (true)
            {
                float puddleDropTime = Random.Range(minPuddleDropTime, maxPuddleDropTime);
                yield return new WaitForSeconds(puddleDropTime);
                while (!SpawnableTarLocation())
                {
                    yield return null;
                }
                toxicPuddleDrop.Spawn(transform.position);
            }
        }

        bool SpawnableTarLocation()
        {
            return (transform.position.x % 1) < maxDistancePuddleSpawn && (transform.position.y % 1) < maxDistancePuddleSpawn && (transform.position.z % 1) < maxDistancePuddleSpawn;
        }
    }
}