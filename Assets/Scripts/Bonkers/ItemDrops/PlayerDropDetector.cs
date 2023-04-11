using UnityEngine;

namespace  Bonkers.Drops
{
    [RequireComponent(typeof(PlayerPickupGrabber))]
    [RequireComponent(typeof(PlayerEnvironmentEffectorGrabber))]
    public class PlayerDropDetector : MonoBehaviour
    {
        PlayerPickupGrabber playerPickupGrabber;
        PlayerEnvironmentEffectorGrabber playerEnvironmentEffectorGrabber;
        
        void Awake()
        {
            playerPickupGrabber = GetComponent<PlayerPickupGrabber>();
            playerEnvironmentEffectorGrabber = GetComponent<PlayerEnvironmentEffectorGrabber>();
        }
        
        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent<IPickupable>(out IPickupable pickupable))
            {
                playerPickupGrabber.AttemptPickup(pickupable);
            }

            if (other.TryGetComponent<IEnvironmentEffector>(out IEnvironmentEffector environmentEffector))
            {
                playerEnvironmentEffectorGrabber.AttemptApplyEffector(environmentEffector);
            }
        }

        void OnTriggerExit2D(Collider2D other)
        {
            if (other.TryGetComponent<IEnvironmentEffector>(out IEnvironmentEffector environmentEffector))
            {
                playerEnvironmentEffectorGrabber.AttemptUnapplyEffector(environmentEffector);
            }
        }
    }
}