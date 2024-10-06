using Bonkers.ItemDrops;
using Bonkers.Movement;
using UnityEngine;

namespace  Bonkers.Control
{
    [RequireComponent(typeof(PlayerEnvironmentEffectorGrabber))]
    public class PlayerEnvironmentEffectorsControl : MonoBehaviour
    {
        PlayerEnvironmentEffectorGrabber playerEnvironmentEffectorGrabber;
        PlayerMovement playerMovement;

        #region  Unity Events/Callbacks

        void Awake()
        {
            playerEnvironmentEffectorGrabber = GetComponent<PlayerEnvironmentEffectorGrabber>();
            playerMovement = GetComponent<PlayerMovement>();
        }

        void OnEnable()
        {
            playerEnvironmentEffectorGrabber.onEnterEnvironmentEffector += PlayerHitEnvironmentalEffector;
            playerEnvironmentEffectorGrabber.onExitEnvironmentEffector += PlayerExitEnvironmentalEffector;
        }
        
        void OnDisable()
        {
            playerEnvironmentEffectorGrabber.onEnterEnvironmentEffector -= PlayerHitEnvironmentalEffector;
            playerEnvironmentEffectorGrabber.onExitEnvironmentEffector -= PlayerExitEnvironmentalEffector;
        }

        #endregion

        void PlayerHitEnvironmentalEffector(ScriptableObject effector)
        {
            PuddleDrop puddle = effector as PuddleDrop;
            if (!puddle) return;

            Debug.Log("Setting move speed slower!");
            playerMovement.SetMoveSpeed(playerMovement.GetMoveSpeed() - puddle.EffectStrength);
        }
        
        void PlayerExitEnvironmentalEffector(ScriptableObject effector)
        {
            PuddleDrop puddle = effector as PuddleDrop;
            if (!puddle) return;
            
            playerMovement.ResetMoveSpeed();
        }
    }

}
