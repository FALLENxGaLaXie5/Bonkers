using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bonkers.Combat;
using Bonkers.Movement;

namespace Bonkers.Control
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerMovement))]
    [RequireComponent(typeof(PlayerCombat))]
    [RequireComponent(typeof(PlayerHealth))]

    public class Player2Control : MonoBehaviour
    {
        #region Inspector and Public Variables
        [SerializeField] [Range(0f, 5f)] float timeBetweenBonks = 0.5f;
        #endregion

        #region Class and Cached Variables
        PlayerMovement mover;
        PlayerCombat combat;
        bool canBonk = true;
        #endregion 

        #region Unity Event Methods
        // Start is called before the first frame update
        void Start()
        {
            mover = GetComponent<PlayerMovement>();
            combat = GetComponent<PlayerCombat>();
        }

        // Update is called once per frame
        void Update()
        {
            HandleMovement();
            if (canBonk && Input.GetKeyDown(KeyCode.Return))
            {
                combat.AttemptBonkBlok();
                StartCoroutine(WaitForBonk());
            }
        }

        #endregion

        #region Class Methods

        /// <summary>
        /// This will handle the 'movePoint' transform in the mover component; will grab input
        ///     if we are at the movepoint
        ///         will check if it can move in the requested direction, either horizontal or vertical, and move the movePoint
        /// 
        /// Player movement will handle actually moving the player towards the movePoint every frame.
        /// </summary>
        void HandleMovement()
        {
            transform.position = Vector3.MoveTowards(transform.position, mover.GetMovePoint().position, mover.GetMoveSpeed() * Time.deltaTime);

            if (Vector3.Distance(transform.position, mover.GetMovePoint().position) <= .05f)
            {
                float inputHorizontal = Input.GetAxisRaw("Horizontal2");
                if (Mathf.Abs(inputHorizontal) == 1f)
                {
                    mover.SetRotation(true, inputHorizontal);
                    if (mover.MovePointHorizontal(inputHorizontal)) return;
                }

                float inputVertical = Input.GetAxisRaw("Vertical2");
                if (Mathf.Abs(inputVertical) == 1f)
                {
                    mover.SetRotation(false, inputVertical);
                    if (mover.MovePointVertical(inputVertical)) return;
                }
            }
        }
        IEnumerator WaitForBonk()
        {
            this.canBonk = false;
            yield return new WaitForSeconds(timeBetweenBonks);
            this.canBonk = true;
        }

        #endregion
    }

}