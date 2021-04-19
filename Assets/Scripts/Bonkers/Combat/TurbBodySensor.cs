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
            turbCombat.HitPlayer(collision.transform);
        }
        if (collision.tag == "Food")
        {
            //invoke the eating food action
            eatFoodAction?.Invoke(collision.transform);
        }
    }
}
