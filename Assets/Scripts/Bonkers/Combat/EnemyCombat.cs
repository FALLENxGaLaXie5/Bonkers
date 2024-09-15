using UnityEngine;

namespace Bonkers.Combat
{
    [DisallowMultipleComponent]
    public class EnemyCombat : MonoBehaviour, IEnemyCombat
    {
        [SerializeField] [Range(1, 10)] int damageAmount = 5;

        IHealth health;

        void Awake() => health = GetComponent<IHealth>();
        
        public void HitPlayer(Transform playerTransform)
        {
            if (playerTransform.TryGetComponent(out PlayerHealth playerHealth))
            {
                //If the player is not currently invincible and AI combat component is enabled, have the player take damage
                // else if the player is invincible 
                if (!playerHealth.IsInvincible() && this.enabled)
                {
                    playerTransform.GetComponent<PlayerHealth>().TakeDamage(damageAmount);
                }
                else if(playerHealth.IsInvincible())
                {
                    //if not invincible and enabled, for now we're just gonna say the player will move down the enemy
                    health.Die();
                }
            }
        }

        public void DisableCombat() => enabled = false;
    }
}

 