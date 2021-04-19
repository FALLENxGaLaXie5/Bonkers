using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bonkers.Combat;
using Bonkers.Movement;

namespace Bonkers.Control
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(IAIMovement))]
    [RequireComponent(typeof(IEnemyCombat))]
    [RequireComponent(typeof(EnemyHealth))]


    public class AIControl : MonoBehaviour
    {

        #region Private/Class Variables
        protected IAIMovement aiMovement;
        protected SpriteRenderer spriteRenderer;

        #endregion

        protected virtual void Awake()
        {
            aiMovement = GetComponent<IAIMovement>();
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        #region Unity Events/Functions
        // Start is called before the first frame update
        protected virtual void Start()
        {
            
        }

        // Update is called once per frame
        protected void Update()
        {
            aiMovement.Patrol();
        }

        #endregion

        public void DisableControl()
        {
            this.enabled = false;
        }
    }
}