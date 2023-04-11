using System;
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
        protected IEnemyCombat combat;
        protected EnemyHealth health;
        
        #endregion

        protected virtual void Awake()
        {
            combat = GetComponent<IEnemyCombat>();
            aiMovement = GetComponent<IAIMovement>();
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            health = GetComponent<EnemyHealth>();
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

        protected void OnEnable() => health.onDisableFunctionality += OnDisableAllControl;

        protected void OnDisable() => health.onDisableFunctionality -= OnDisableAllControl;

        #endregion

        public void DisableControl()
        {
            enabled = false;
        }

        void OnDisableAllControl()
        {
            combat.DisableCombat();
            aiMovement.DisableMovement();
            DisableControl();
        }
    }
}