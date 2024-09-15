using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bonkers.Combat;

public class TurbBodySensor : MonoBehaviour
{

    public bool isEnabled = false;
    EnemyCombat _enemyCombat;

    public event Action<Transform> eatFoodAction;

    void Start()
    {
        _enemyCombat = transform.parent.GetComponent<EnemyCombat>();
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        //sense if enemy hit the player
        if (collision.tag == "Player" && isEnabled)
        {
            if (!_enemyCombat)
            {
                if(transform.parent.TryGetComponent<EnemyCombat>(out EnemyCombat combat))
                    combat.HitPlayer(collision.transform);
                else
                    _enemyCombat.HitPlayer(collision.transform);
            }
            else
            {
                _enemyCombat.HitPlayer(collision.transform);
            }
        }
        if (collision.tag == "Food")
        {
            //invoke the eating food action
            eatFoodAction?.Invoke(collision.transform);
        }
    }
}
