using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bonkers.ItemDrops;

namespace Bonkers.Control
{
    public class TarSlimoControl : AIControl
    {
        [SerializeField] PuddleDrop tarDrop;
        [SerializeField] [Range(1f, 15f)] float minPuddleDropTime = 3f;
        [SerializeField] [Range(3f, 30f)]float maxPuddleDropTime = 7f;
        [SerializeField] [Range(0.01f, 0.3f)] float maxDistancePuddleSpawn = 0.02f;

        new void Start()
        {
            base.Start();
            StartCoroutine(DropPuddles());
        }

        new void Update()
        {
            base.Update();
        }

        IEnumerator DropPuddles()
        {
            while (true)
            {
                float puddleDropTime = Random.Range(minPuddleDropTime, maxPuddleDropTime);
                yield return new WaitForSeconds(puddleDropTime);
                while(!SpawnableTarLocation())
                {
                    yield return null;
                }
                tarDrop.Spawn(transform.position);
            }
        }

        bool SpawnableTarLocation()
        {
            return (transform.position.x % 1) < maxDistancePuddleSpawn && (transform.position.y % 1) < maxDistancePuddleSpawn && (transform.position.z % 1) < maxDistancePuddleSpawn;
        }
    }
}