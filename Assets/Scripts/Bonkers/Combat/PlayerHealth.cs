using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bonkers.Combat
{
    [DisallowMultipleComponent]
    public class PlayerHealth : MonoBehaviour, IHealth
    {
        #region Public and Inspector Variables

        [SerializeField] bool invincible = false;
        public int health = 5;
        public event Action onPlayerDeath;

        #endregion

        #region Class and Private Variables

        AudioSource audioSource;

        #endregion

        #region Unity Events
        void Start()
        {
            audioSource = GetComponentInChildren<AudioSource>();        
        }

        #endregion

        #region Class Functions

        public void TakeDamage(int damage)
        {
            //invincible is for testing purposes if we don't want player to be able to die
            if (invincible) return;

            this.health -= damage;
            if (this.health <= 0)
            {
                Die();
            }
        }

        public void Die()
        {
            Transform playersParentTransform = transform.parent;
            transform.parent = null;
            
            audioSource.transform.parent = null;
            audioSource.Play();
            Destroy(audioSource.transform.gameObject, 2f);
            onPlayerDeath?.Invoke();
            Destroy(gameObject);
        }

        public void SetInvincible(bool isInvincible)
        {
            invincible = isInvincible;
        }

        public bool IsInvincible()
        {
            return invincible;
        }

        #endregion
    }
}