using System;
using System.Collections;
using System.Collections.Generic;
using Bonkers.Animation;
using UnityEngine;
using Bonkers.Combat;
using Bonkers.Movement;

namespace Bonkers.Control
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(IAIMovement))]
    [RequireComponent(typeof(IEnemyCombat))]
    [RequireComponent(typeof(EnemyHealth))]
    [RequireComponent(typeof(EnemyAnimation))]
    public abstract class AIControl : MonoBehaviour
    {

        #region Private/Class Variables
        protected IAIMovement aiMovement;
        protected SpriteRenderer spriteRenderer;
        protected IEnemyCombat combat;
        protected EnemyHealth health;
        protected EnemyAnimation animation;
        protected ActivateDumbAIBrain aiBrainActivationObject;
        
        #endregion

        protected virtual void Awake()
        {
            combat = GetComponent<IEnemyCombat>();
            aiMovement = GetComponent<IAIMovement>();
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            health = GetComponent<EnemyHealth>();
            aiBrainActivationObject = GetComponentInChildren<ActivateDumbAIBrain>();
            animation = GetComponent<EnemyAnimation>();
            health.OnDisableFunctionality += OnDisableAllControl;
        }

        #region Unity Events/Functions
        // Start is called before the first frame update
        protected virtual void Start()
        {
            
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            aiMovement.Patrol();
        }
        
        protected virtual void OnDisable() => health.OnDisableFunctionality -= OnDisableAllControl;

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