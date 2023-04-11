using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bonkers.Drops
{
    public class PowerUpPickup : MonoBehaviour, IPickupable
    {
        #region Inspector/Public Variables

        //PowerUp scriptable object - contains data for the powerup
        [SerializeField] Powerup powerup = null;

        [SerializeField] float staminaBoost = 0.5f;

        #endregion
        
        #region Class Functions

        public ScriptableObject AttemptPickup() => powerup;

        public void DestroyPickupGameObject(float timeToDestroy) => Destroy(gameObject, 0f);
        
        #endregion
    }

}