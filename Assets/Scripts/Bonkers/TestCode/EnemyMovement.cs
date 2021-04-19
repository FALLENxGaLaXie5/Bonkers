using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{

    [SerializeField] float movementPeriod = .5f;
    [SerializeField] float waitBetweenPaths = 1f;
    //[SerializeField] ParticleSystem goalParticle;

    


    PathfindingSearch pathfinder;
    // Use this for initialization
    void Start()
    {
        pathfinder = GetComponent<PathfindingSearch>();
        StartCoroutine(FollowPath(pathfinder.SetNewRandomPath()));
    }

    IEnumerator FollowPath(List<Waypoint> path)
    {
        while (true)
        {
            foreach (Waypoint waypoint in path)
            {
                transform.position = waypoint.transform.position;
                yield return new WaitForSeconds(movementPeriod);
            }
            yield return new WaitForSeconds(waitBetweenPaths);
            path = pathfinder.SetNewRandomPath();
        }

    }

}