using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bonkers.Combat;

public class TurbBodySensor : MonoBehaviour
{

    public bool isEnabled = false;
    TurbCombat turbCombat;

    public event Action<Transform> eatFoodAction;

    void Start()
    {
        turbCombat = transform.parent.GetComponent<TurbCombat>();
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        //sense if enemy hit the player
        if (collision.tag == "Player" && isEnabled)
        {
            if (!turbCombat)
            {
                if(transform.parent.TryGetComponent<TurbCombat>(out TurbCombat combat))
                    combat.HitPlayer(collision.transform);
                else
                    turbCombat.HitPlayer(collision.transform);
            }
            else
            {
                turbCombat.HitPlayer(collision.transform);
            }
        }
        if (collision.tag == "Food")
        {
            //invoke the eating food action
            eatFoodAction?.Invoke(collision.transform);
        }
    }
}
