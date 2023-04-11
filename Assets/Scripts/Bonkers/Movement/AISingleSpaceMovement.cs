using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Bonkers.Movement
{
    public class AISingleSpaceMovement : MonoBehaviour, IAIMovement
    {
        #region Public/Inspector Variables
        [SerializeField][Range(1, 90)][Tooltip("This will be the percentage chance that the AI will change movement directions when moving forward and no objects encountered.")]
        float percentChanceChangeDir = 25f;
        [SerializeField][Range(0f, 10f)][Tooltip("Movement speed for space by space AI")]
        float speed = 3f;
        [SerializeField][Range(0.05f, 0.5f)][Tooltip("How much time it will take to rotate AI")]
        float rotationTime = 0.2f;
        [SerializeField][Range(0.005f, 0.05f)][Tooltip("Distance to check if at new space")]
        float checkDistance = 0.02f;
        [SerializeField][Range(0.1f, 0.6f)][Tooltip("If all adjacent spaces taken, time to recheck to move")]
        float recheckWaitTimeIfStuck = 0.3f;
        [SerializeField] float minRotateWaitTime = 0.5f;
        [SerializeField] float maxRotateWaitTime = 1.5f;
        [SerializeField] Transform movePoint;

        public LayerMask cannotPatrolMask = new LayerMask();


        #endregion

        #region Private/Class Variables

        Vector3 facingDir = Vector3.down;
        Vector3 previousMovepointPos = Vector3.zero;
        Dictionary<Vector3, float> directionsToZEulerDict = new Dictionary<Vector3, float>();
        bool canTurn = true;
        bool movementDisabled = false;
        float rotateWaitTime = 1.0f;

        #endregion

        #region Unity Event Functions

        void Start()
        {
            movePoint.parent = null;
            PopulateDirectionsToZEulerDict();
            transform.rotation = Quaternion.Euler(0, 0, directionsToZEulerDict[facingDir]);
            rotateWaitTime = UnityEngine.Random.Range(minRotateWaitTime, maxRotateWaitTime);
            previousMovepointPos = movePoint.position;
        }

        #endregion

        #region Class Functions

        public void Patrol()
        {
            //If movement is disabled, don't move at all
            if (movementDisabled) return;

            transform.position = Vector3.MoveTowards(transform.position, movePoint.position, speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, movePoint.position) <= checkDistance)
            {
                //If we need to change directions, it will pick a new direction and then set the movepoint ahead in that direction
                if (ShouldChangeDirection())
                {
                    StartCoroutine(ChooseNewDirection());
                }
                else if(!Physics2D.OverlapCircle(transform.position + facingDir, 0.1f, cannotPatrolMask))
                {
                    //If we do not need to change directions, it will just set the movepoint forward in the same direction
                    MoveMovepoint(facingDir);
                }
            }
        }

        bool ShouldChangeDirection()
        {
            //Some percent chance of changing direction every time AI gets to new space or there is an object in the AI's path
            return ((canTurn && UnityEngine.Random.Range(1, 100) < percentChanceChangeDir) || (canTurn && Physics2D.OverlapCircle(transform.position + facingDir, 0.1f, cannotPatrolMask)));
        }

        IEnumerator ChooseNewDirection()
        {            
            if (canTurn)
            {
                bool ableMove = false;

                //This will just be used to keep track of what directions have already been checked
                Queue<Vector3> directions = RefillDirectionsList();

                while (!ableMove)
                {
                    if (directions.Count <= 0)
                    {
                        yield return new WaitForSeconds(recheckWaitTimeIfStuck);
                        directions = RefillDirectionsList();
                    }
                    Vector3 newDir = directions.Dequeue();

                    //If there is nothing one space over in the new direction
                    if (!Physics2D.OverlapCircle(transform.position + newDir, 0.1f, cannotPatrolMask))
                    {
                        SetFacingDir(newDir);

                        //Round new movepoint position to ensure enemy position doesn't veer off from snapping onto grid positions
                        MoveMovepoint(facingDir);
                        ableMove = true;
                    }
                    //If that space has something in it, will keep looping (Dequeue already took that dir out of the Queue)
                }
            }            
        }

        void MoveMovepoint(Vector3 newFacingDirection)
        {
            previousMovepointPos = movePoint.position;
            movePoint.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), 0) + newFacingDirection;            
        }

        void ResetToPreviousMovePointPos()
        {
            Vector3 copyOfMovepointPos = new Vector3(movePoint.position.x, movePoint.position.y, 0);
            Vector3 copyOfPreviousPos = new Vector3(previousMovepointPos.x, previousMovepointPos.y, 0);
            previousMovepointPos = copyOfMovepointPos;
            movePoint.position = copyOfPreviousPos;
        }

        Queue<Vector3> RefillDirectionsList()
        {
            Queue<Vector3> directions = new Queue<Vector3>();

            Vector3 rightRelative = new Vector3(facingDir.y, -facingDir.x, 0f);
            Vector3 leftRelative = -rightRelative;

            if(UnityEngine.Random.Range(0, 2) == 0)
            {
                //Add left first, then right
                directions.Enqueue(leftRelative);
                directions.Enqueue(rightRelative);
            }
            else
            {
                //Add right first, then left
                directions.Enqueue(rightRelative);
                directions.Enqueue(leftRelative);
            }

            //Add in forward facingDir and backward facingDir
            directions.Enqueue(-facingDir);
            directions.Enqueue(facingDir);

            return directions;
        }

        void PopulateDirectionsToZEulerDict()
        {
            directionsToZEulerDict.Add(Vector3.down, 180);
            directionsToZEulerDict.Add(Vector3.right, 270);
            directionsToZEulerDict.Add(Vector3.up, 0);
            directionsToZEulerDict.Add(Vector3.left, 90);
        }

        public Vector3 GetFacingDir() => facingDir;

        void SetFacingDir(Vector3 newFacingDir)
        {
            facingDir = newFacingDir;
            transform.DORotate(new Vector3(0, 0, directionsToZEulerDict[facingDir]), rotationTime);
            StartCoroutine(TimeToNextAbleRotate());
        }

        IEnumerator TimeToNextAbleRotate()
        {
            canTurn = false;
            yield return new WaitForSeconds(rotateWaitTime);
            canTurn = true;
        }

        public void BackTrackPath(Vector3 currentFacingDir)
        {
            if (!canTurn) return;
            SetFacingDir(-facingDir);
            ResetToPreviousMovePointPos();
            //If no other direction than forward is clear, continue forward
        }

        public void DisableMovement()
        {
            movementDisabled = true;
        }

        public Transform GetMovePoint()
        {
            return movePoint;
        }
        
        #endregion

    }
}