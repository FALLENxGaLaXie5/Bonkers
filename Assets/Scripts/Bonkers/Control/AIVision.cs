using Bonkers.Movement;
using System.Collections;
using System.Collections.Generic;
using Bonkers.Combat;
using UnityEngine;

namespace Bonkers.Control
{
    public class AIVision : MonoBehaviour
    {
        #region Public/Inspector Variables

        [SerializeField] float minVisionDistance = 0.03f;
        [SerializeField] float maxVisionDistance = 1.0f;
        [SerializeField] BoxCollider2D visionCollider;

        public Vector2 facingDir;

        #endregion

        #region Unity Event Functions

        void Start()
        {
            visionCollider.offset = new Vector2(visionCollider.offset.x, UnityEngine.Random.Range(minVisionDistance, maxVisionDistance));
        }

        // Update is called once per frame
        void Update()
        {
            facingDir = GetCurrentFacingDirection();
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            //Top parent object will be tagged as "Enemy", and the child body sensor will have the TurbBodySensor component on it (not tagged as enemy so other tag uses don't conflict)
            if (collision.transform.tag == "Enemy" || collision.transform.GetComponent<TurbBodySensor>())
            {               
                //transform.parent.GetComponent<IAIMovement>().BackTrackPath(GetCurrentFacingDirection());
            }
        }

        #endregion

        #region Class Functions

        Vector3 GetCurrentFacingDirection()
        {
            return new Vector3(Mathf.Round(transform.parent.up.x), Mathf.Round(transform.parent.up.y), 0);
        }

        #endregion
    }
}

