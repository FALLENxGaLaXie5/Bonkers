using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bonkers.Core;
using Bonkers.Combat;

namespace Bonkers.BlokControl
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(IBlokInteraction))]
    public class WoodenBoxControl : MonoBehaviour, IBlokControl
    {
        [SerializeField] float moveSpeed = 0f;
        bool isMoving = false;
        Vector3 moveDir = Vector3.zero;
        AudioSource breakingSound;

        void Start()
        {
            breakingSound = GetComponent<AudioSource>();
        }

        public void SetMoving(bool newIsMoving, Vector3 movementDir)
        {
            this.isMoving = newIsMoving;
            this.moveDir = movementDir;
        }

        public bool IsMoving()
        {
            return this.isMoving;
        }

        public void PlaySound()
        {
            breakingSound.Play();
        }

        public void HitEnemy(Transform enemyTransform)
        {

        }

        public float GetCurrentSpeed()
        {
            return this.moveSpeed;
        }

    }
}
