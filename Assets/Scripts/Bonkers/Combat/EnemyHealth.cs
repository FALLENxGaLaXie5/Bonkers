using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bonkers.Movement;
using Bonkers.Control;
using DG.Tweening;
using Pathfinding;

namespace Bonkers.Combat
{
    [DisallowMultipleComponent]
    public class EnemyHealth : MonoBehaviour, IHealth
    {
        #region Inspector/Public Variables
        [SerializeField] int scoreValue = 100;
        [SerializeField] GameObject scoreObject;
        [SerializeField] float scoreObjectSpawnOffset = 1f;
        [SerializeField] ParticleSystem slimeExplosion = new ParticleSystem();
        [SerializeField][Range(0.1f, 5f)] float timeBetweenHits = 1f;
        [SerializeField] [Range(0.01f, 2f)] float pauseBeforeDestroy = 0.2f;
        [SerializeField] float dissolveSpeed = 2f;
        
        #endregion

        #region Private/Class Variables
        int health = 5;
        bool canBeHit = true;
        float dissolveModifierValue = 1f;
        Tween dissolveTween = null;
        Material spriteMaterial;
        IEnemyCombat combat;
        AIControl control;
        IAIMovement movement;
        Rigidbody2D rigidBody;
        #endregion

        #region Unity Events/Methods

        void Awake()
        {
            spriteMaterial = GetComponentInChildren<SpriteRenderer>().material;
            combat = GetComponent<IEnemyCombat>();
            control = GetComponent<AIControl>();
            movement = GetComponent<IAIMovement>();
            rigidBody = GetComponent<Rigidbody2D>();
        }

        void Update()
        {
            //if dissolveTween has started
            if (dissolveTween != null && dissolveTween.IsPlaying())
            {
                if (!spriteMaterial) return;
                //if sprite material is set, need to set fade value
                spriteMaterial.SetFloat("_Fade", dissolveModifierValue);
            }
        }

        #endregion

        #region Class Functions
        public void TakeDamage(int damage)
        {
            if (canBeHit)
            {
                StartCoroutine(PauseHittable());
                SetHealth(health - damage);
                if (health <= 0) Die();
            }
        }

        /// <summary>
        /// Will kill off enemy; increments score, plays any sort of VFX, dissolves enemy if that shader is on, disables functionality, 
        /// and destroys enemy when that's all done
        /// </summary>
        public void Die()
        {
            //dissolve enemy - need to actually set shader fade in update
            dissolveTween = DOTween.To(() => dissolveModifierValue, x => dissolveModifierValue = x, 0f, dissolveSpeed);

            //this will disable the control, combat, and movement classes and disable any kind of AI Functionality
            DisableAllAIFunctionality();

            //will wait until dissolve is complete to kill enemy object
            StartCoroutine(DestroyEnemyObject(dissolveTween));

            //increment corresponding score
            GameObject scoreTextObject = Instantiate(scoreObject, this.transform.position + new Vector3(0f, scoreObjectSpawnOffset, 0f), Quaternion.identity);
            scoreTextObject.GetComponentInChildren<TextMesh>().text = this.scoreValue.ToString();
            
            //set this flag so that Die can't be called multiple times
            canBeHit = false;

            //play then destroy explosion particle effect
            slimeExplosion.transform.parent = null;
            slimeExplosion.Play();
            Destroy(slimeExplosion.gameObject, pauseBeforeDestroy);            
        }

        void DisableAllAIFunctionality()
        {
            combat.DisableCombat();
            control.DisableControl();
            movement.DisableMovement();

            //this will disable any forces acting on rigidBody (blok's exploding were exuding force on it on death)
            rigidBody.isKinematic = true;
        }

        IEnumerator DestroyEnemyObject(Tween dissolveTween)
        {
            yield return dissolveTween.WaitForCompletion();            
            Destroy(this.gameObject);
        }

        IEnumerator PauseHittable()
        {
            this.canBeHit = false;
            yield return new WaitForSeconds(timeBetweenHits);
            this.canBeHit = true;
        }

        public int GetHealth()
        {
            return this.health;
        }

        public void SetHealth(int newHealth)
        {
            this.health = newHealth;
        }

        public int GetScoreValue()
        {
            return this.scoreValue;
        }

        #endregion
    }
}

