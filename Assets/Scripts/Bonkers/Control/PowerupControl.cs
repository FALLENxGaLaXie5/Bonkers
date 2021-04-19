using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Bonkers.Drops;
using Bonkers.Combat;
using Bonkers.Movement;
using System;

namespace Bonkers.Control
{
    public class PowerupControl : MonoBehaviour
    {

        #region Inspector/Public Variables

        [SerializeField][Range(0.3f, 10f)] float shieldDuration = 10f;
        [SerializeField] float initialShieldValue = 0.1f;
        [SerializeField][Range(0.3f, 3f)] float shieldLoopBeginThickness = 1f;        
        [SerializeField] [Range(0.3f, 3f)] float shieldLoopEndThickness = 20f;
        [SerializeField] [Range(1, 20)] int totalShieldLoops = 5;
        [SerializeField] string materialValueModifierName = "_OutlineThickness";
        [SerializeField] string materialColorModifierName = "_OutlineColor";
        [SerializeField] Ease _moveEase = Ease.Linear;



        #endregion

        #region Private/Class Variables

        PlayerHealth playerHealth;
        PlayerMovement playerMovement;
        [SerializeField] Powerup currentPowerup;
        Material shieldMat;
        Coroutine powerupRoutine;
        
        float materialModifierValue = 0f;

        #endregion

        #region Unity Event Functions

        void Awake()
        {
            shieldMat = GetComponent<SpriteRenderer>().material;
            playerHealth = GetComponent<PlayerHealth>();
            playerMovement = GetComponent<PlayerMovement>();
        }

        void Update()
        {
            //if current powerup is null, don't do anything
            if (!currentPowerup) return;

            SetSingleMaterialValue(materialValueModifierName, materialModifierValue);    
        }

        #endregion

        #region Class Functions

        /// <summary>
        /// Will be called from the powerup pick-up object. Sets player as invincible and starts tweening the shield effect
        /// </summary>
        /// <param name="newPowerup"></param>
        public void ActivateShield(Powerup newPowerup)
        {
            if(currentPowerup) DeactivateAnyPowerup();


            //for now, shield will just set player health invincible variable to true/false when activated/deactivated. Enemies will check that variable on collision
            playerHealth.SetInvincible(true);

            //set current powerup to shield powerup
            SetCurrentPowerup(newPowerup);

            //tweens the shield animation with the shield shader
            Sequence myShieldSequence = DOTween.Sequence();
            myShieldSequence.Append(DOTween.To(() => materialModifierValue, x => materialModifierValue = x, shieldLoopBeginThickness, shieldDuration / 4));
            myShieldSequence.Append(DOTween.To(() => materialModifierValue, x => materialModifierValue = x, shieldLoopEndThickness, (shieldDuration / 2) / totalShieldLoops / 2).SetLoops(totalShieldLoops * 2, LoopType.Yoyo));

            Tween finalShieldTween = DOTween.To(() => materialModifierValue, x => materialModifierValue = x, initialShieldValue, shieldDuration / 4);
            myShieldSequence.Append(finalShieldTween);

            //will wait until final shield tween effect is complete to make player killable again - so effect and logic are in sync
            powerupRoutine = StartCoroutine(WaitToDeactivateShield(finalShieldTween));
        }

        public void ActivateStamina(Powerup newPowerup)
        {
            if (currentPowerup) DeactivateAnyPowerup();

            //set player to recharge stamina + boosted rate
            playerMovement.StaminaEffect(newPowerup.GetModifier());

            //set current powerup to shield powerup
            SetCurrentPowerup(newPowerup);

            //tweens the shield animation with the shield shader
            Sequence myStaminaSequence = DOTween.Sequence();
            myStaminaSequence.Append(DOTween.To(() => materialModifierValue, x => materialModifierValue = x, shieldLoopBeginThickness, shieldDuration / 4));
            myStaminaSequence.Append(DOTween.To(() => materialModifierValue, x => materialModifierValue = x, shieldLoopEndThickness, (shieldDuration / 2) / totalShieldLoops / 2).SetLoops(totalShieldLoops * 2, LoopType.Yoyo));

            Tween finalStaminaTween = DOTween.To(() => materialModifierValue, x => materialModifierValue = x, initialShieldValue, shieldDuration / 4);
            myStaminaSequence.Append(finalStaminaTween);

            powerupRoutine = StartCoroutine(WaitToDeactivateStamina(finalStaminaTween));
        }

        /// <summary>
        /// Will wait until final shield tween effect is complete to make player killable again - so effect and logic are in sync
        /// </summary>
        /// <param name="shieldTween"></param>
        /// <returns></returns>
        IEnumerator WaitToDeactivateShield(Tween shieldTween)
        {
            //need to reset powerup after final shield tween effect is complete
            yield return shieldTween.WaitForCompletion();
            DeactivateShield();
        }

        IEnumerator WaitToDeactivateStamina(Tween staminaTween)
        {
            yield return staminaTween.WaitForCompletion();
            DeactivateStaminaBoost();
        }

        void DeactivateShield()
        {
            playerHealth.SetInvincible(false);
            currentPowerup = null;
        }

        void DeactivateStaminaBoost()
        {
            playerMovement.StopStaminaEffect(currentPowerup.GetModifier());
            currentPowerup = null;
        }

        void DeactivateAnyPowerup()
        {
            //will reset any coroutine that may be running on powerup
            if (powerupRoutine != null) StopCoroutine(powerupRoutine);

            //will need to individually run deactivation methods for each type of powerup here
            DeactivateShield();
            DeactivateStaminaBoost();
        }

        void SetSingleMaterialValue(string fieldToSet, float newValue)
        {
            shieldMat.SetFloat(fieldToSet, newValue);            
        }

        void SetMaterialColor(string fieldToSet, Powerup newPowerup)
        {
            shieldMat.SetColor(fieldToSet, newPowerup.GetColor());
        }

        void SetCurrentPowerup(Powerup newPowerup)
        {
            currentPowerup = newPowerup;
            //set the material color as well for player
            SetMaterialColor(materialColorModifierName, newPowerup);

            //reset the material modifier value used to lerp the effect edge
            materialModifierValue = 0f;
        }

        #endregion
    }
}