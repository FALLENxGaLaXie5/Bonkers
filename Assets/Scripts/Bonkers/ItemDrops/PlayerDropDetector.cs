using UnityEngine;

namespace  Bonkers.ItemDrops
{
    [RequireComponent(typeof(PlayerPickupGrabber))]
    [RequireComponent(typeof(PlayerEnvironmentEffectorGrabber))]
    public class PlayerDropDetector : MonoBehaviour
    {
        private PlayerPickupGrabber _playerPickupGrabber;
        private PlayerEnvironmentEffectorGrabber _playerEnvironmentEffectorGrabber;
        
        void Awake()
        {
            _playerPickupGrabber = GetComponent<PlayerPickupGrabber>();
            _playerEnvironmentEffectorGrabber = GetComponent<PlayerEnvironmentEffectorGrabber>();
        }
        
        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out IPickupable pickupable))
            {
                _playerPickupGrabber.AttemptPickup(pickupable);
            }

            if (other.TryGetComponent(out IEnvironmentEffector environmentEffector))
            {
                _playerEnvironmentEffectorGrabber.AttemptApplyEffector(environmentEffector);
            }
        }

        void OnTriggerExit2D(Collider2D other)
        {
            if (other.TryGetComponent(out IEnvironmentEffector environmentEffector))
            {
                _playerEnvironmentEffectorGrabber.AttemptUnapplyEffector(environmentEffector);
            }
        }
    }
}