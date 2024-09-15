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

        private ActivateDumbAIBrain aiBrainActivationObject;
        
        #endregion

        protected virtual void Awake()
        {
            combat = GetComponent<IEnemyCombat>();
            aiMovement = GetComponent<IAIMovement>();
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            health = GetComponent<EnemyHealth>();
            aiBrainActivationObject = GetComponentInChildren<ActivateDumbAIBrain>();
            
            health.OnDisableFunctionality += OnDisableAllControl;
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
        
        protected void OnDisable() => health.OnDisableFunctionality -= OnDisableAllControl;

        #endregion

        public void DisableControl() => enabled = false;

        void OnDisableAllControl()
        {
            aiBrainActivationObject.DisableControlActivation();
            combat.DisableCombat();
            aiMovement.DisableMovement();
            DisableControl();
        }
    }
}