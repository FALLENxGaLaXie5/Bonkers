using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System;
using Bonkers.Grid;


namespace Bonkers.Movement
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Seeker))]
    [RequireComponent(typeof(AIDestinationSetter))]
    [RequireComponent(typeof(AILerp))]
    public class AIPathfinderMovement : MonoBehaviour, IAIMovement
    {

        #region Public/Inspector Variables
        [SerializeField] float endOfPathDistance = 0.05f;

        public LayerMask cannotPatrolMask = new LayerMask();
        
        #endregion

        #region Private/Class Variables

        PatrolPoints points;

        public Transform backTrackTarget;
        Seeker seeker;
        AILerp aiLerp;
        AIDestinationSetter destinationSetter;
        Transform helperTargetStorage;

        #endregion

        #region Unity Event Functions

        void Awake()
        {
            aiLerp = GetComponent<AILerp>();
            destinationSetter = GetComponent<AIDestinationSetter>();
            seeker = GetComponent<Seeker>();
        }

        // Start is called before the first frame update
        void Start()
        {
            backTrackTarget = transform.Find("BackTrackTarget");
            helperTargetStorage = GameObject.FindGameObjectWithTag("HelperTargetStorage").transform;
            CachePatrolPointsObject();
        }

        #endregion


        #region Class Functions

        public void Patrol()
        {
            //Check to make sure target is not null
            if (destinationSetter.target)
            {
                if (Vector3.Distance(transform.position, destinationSetter.target.position) <= endOfPathDistance)
                {
                    //ChooseNewPatrolTarget will also assign new target to the destination setter if using AILerp or the destination in AstarAI class
                    Transform newTarget = ChooseNewPatrolTarget();
                    CalculateAndCallNewPath(newTarget);
                }
            }
            else
            {
                Debug.LogError("Where's the target??? Make sure Grid object is set up with tag 'Grid' and script 'Patrol Points'");
            }
        }

        /// <summary>
        /// Based on the parameter "currentFacingDir", will set the transform of the class variable "backTrackTarget" to the furthest
        ///     back space it can raycast to. Will then start a path to it.
        /// </summary>
        /// <param name="currentFacingDir"></param>
        public void BackTrackPath(Vector3 currentFacingDir)
        {
            //if backTrackTarget has been destroyed, don't execute
            if (!backTrackTarget) return;

            Vector3 currentPos = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), 0) - currentFacingDir;

            bool foundBackTrackPos = false;
            while (!foundBackTrackPos)
            {
                if (Physics2D.OverlapCircle(currentPos, 0.2f, cannotPatrolMask))
                {
                    foundBackTrackPos = true;
                    currentPos += currentFacingDir;
                    backTrackTarget.position = currentPos;
                    //Find new path to back track target now
                    // If we keep the parent as the AI object, whenever it turns it will turn the backTrack transform with it. Need to set parent to null
                    SetHelperTargetInStorage();
                    destinationSetter.target = backTrackTarget;
                    CalculateAndCallNewPath(backTrackTarget);
                    return;
                }
                else
                {
                    currentPos -= currentFacingDir;
                }
            }
        }


        public void SetMovementTarget(Transform target)
        {
            destinationSetter.target = target;
            CalculateAndCallNewPath(target);
        }

        public Transform ChooseNewPatrolTarget()
        {
            Transform possibleTarget = null;
            if (points)
            {
                possibleTarget = points.patrolPoints[UnityEngine.Random.Range(0, points.patrolPoints.Count)];
                while (Physics2D.OverlapCircle(possibleTarget.position, 0.2f, cannotPatrolMask))
                {
                    possibleTarget = points.patrolPoints[UnityEngine.Random.Range(0, points.patrolPoints.Count)];
                }

                SetMovementTarget(possibleTarget);
                backTrackTarget.position = transform.position;
                ResetHelperTarget();
            }
            else
            {
                Debug.LogError("WHERE IS PATROL POINTS OBJECT? NEED GRID OBJECT TAGGED 'Grid' WITH Patrol Points SCRIPT ON IT!");
            }
            return possibleTarget;
        }

        void CachePatrolPointsObject()
        {
            points = GameObject.FindGameObjectWithTag("Grid").transform.GetComponent<PatrolPoints>();
            if (!points) Debug.LogError("PATROL POINTS WAS NOT FOUND IN SCENE: NEED TO ENSURE GRID OBJECT TAGGED AS 'Grid' IS IN SCENE");
        }

        void CalculateAndCallNewPath(Transform newTarget)
        {
            // Create a new Path object
            ABPath path = ABPath.Construct(transform.position, newTarget.position, null);
            seeker.StartPath(path, OnPathComplete);
        }

        void OnPathComplete(Path p)
        {
            //Debug.Log("A path was calculated. Did it fail with an error? " + p.error);
            if (!p.error)
            {
                // Reset the waypoint counter so that we start to move towards the first point in the path
            }
        }

        void SetHelperTargetInStorage()
        {
            if (helperTargetStorage)
                backTrackTarget.parent = helperTargetStorage;
            else
                backTrackTarget.parent = null;
        }

        void ResetHelperTarget()
        {
            backTrackTarget.parent = this.transform;
        }

        public AIDestinationSetter GetDestinationSetterObject()
        {
            return this.destinationSetter;
        }

        public void DestroyHelperTarget()
        {
            if (!backTrackTarget) return;
            Destroy(backTrackTarget.gameObject);
        }

        public void DisableMovement()
        {
            DestroyHelperTarget();
            if (TryGetComponent<AILerp>(out AILerp movement))
            {
                //if there is an AILerp component (there should be for all enemies if they are using a* pathfinding)
                movement.speed = 0f;
                movement.enabled = false;
            }
            enabled = false;
        }

        #endregion
    }


}
