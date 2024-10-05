using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Bonkers.Combat
{
    public class TurbBodySensor : MonoBehaviour
    {
        [SerializeField] private EnemyCombat enemyCombat;
        public event Action<Transform> OnEatFood;
        public bool isEnabled;

        void OnTriggerEnter2D(Collider2D collision)
        {
            //sense if enemy hit the player
            if (collision.CompareTag("Player") && isEnabled)
                enemyCombat.HitPlayer(collision.transform);
            if (collision.CompareTag("Food"))
                OnEatFood?.Invoke(collision.transform);
        }
    }
}
