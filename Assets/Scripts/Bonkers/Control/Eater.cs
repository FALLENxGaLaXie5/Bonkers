﻿using System;
using System.Collections;
using System.Collections.Generic;
using Bonkers.Combat;
using UnityEngine;

namespace Bonkers.Control
{
    public class Eater : MonoBehaviour
    {
        #region Class Variables

        TurbBodySensor bodySensor;
        GrubberControl grubberControl;

        #endregion

        #region Unity Event Functions

        void Awake()
        {
            grubberControl = GetComponent<GrubberControl>();
            bodySensor = GetComponentInChildren<TurbBodySensor>();
        }

        void OnEnable()
        {
            //listen for a collision with food
            bodySensor.OnEatFood += OnEatFood;
        }

        void OnDisable()
        {
            //stop listening for a collision with food
            bodySensor.OnEatFood -= OnEatFood;
        }

        #endregion

        #region Class Functions

        void OnEatFood(Transform food)
        {
            //already checked for tag in the "Turb Body Sensor", so just eat the food!
            grubberControl.SetChasingFood(false);
            Destroy(food.gameObject);
        }

        #endregion
    }
}