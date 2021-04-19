using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bonkers.Control;

namespace Bonkers.Drops
{
    public class PowerupPickup : MonoBehaviour
    {
        #region Inspector/Public Variables

        [SerializeField] Powerup powerup = null;

        [SerializeField] float staminaBoost = 0.5f;

        #endregion

        #region Private/Class Variables

        #endregion

        #region Unity Event Functions

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.tag != "Player") return;
  
            //if it's the player this object is colliding with, then pick up whatever powerup is on this object
            Pickup(other.GetComponent<PowerupControl>());
        }

        #endregion

        #region Class Functions

        void Pickup(PowerupControl powerupControl)
        {
            print("Activate powerup!");
            switch(powerup.GetName())
            {
                case "shield":
                    powerupControl.ActivateShield(powerup);
                    break;
                case "stamina":
                    powerupControl.ActivateStamina(powerup);
                    break;
                default:
                    return;
            }
            
            Destroy(gameObject);
        }

        #endregion
    }

}