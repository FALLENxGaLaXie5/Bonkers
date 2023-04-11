using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bonkers.Movement;

namespace Bonkers.Control
{
    [RequireComponent(typeof(Eater))]
    public class GrubberControl : AIControlPathfinder
    {
        [SerializeField] float smellTime = 3f;
        public Transform foodDrops;
        public bool chasingFood = false;       

        // Start is called before the first frame update
        new void Start()
        {
            base.Start();
            foodDrops = GameObject.FindWithTag("FoodDrops").transform;
            StartCoroutine(StartSmelling());
        }

        // Update is called once per frame
        new void Update()
        {
            base.Update();
            UpdateColor();
        }

        public void SetChasingFood(bool newIsChasingFood)
        {
            if (!newIsChasingFood)
            {
                moverPathfinder.ChooseNewPatrolTarget();
            }
            this.chasingFood = newIsChasingFood;
        }

        IEnumerator StartSmelling()
        {
            while (true)
            {
                yield return new WaitForSeconds(smellTime);
                SmellForFood();
            }            
        }

        void SmellForFood()
        {
            if (foodDrops.childCount > 0)
            {
                Transform closestFood = GetClosestFood();
                chasingFood = true;
                moverPathfinder.SetMovementTarget(closestFood);
            }
            else
            {
                chasingFood = false;
            }
        }

        void UpdateColor()
        {
            if (chasingFood) spriteRenderer.color = Color.red;
            else spriteRenderer.color = Color.white;
        }

        Transform GetClosestFood()
        {
            float closestDistance = Mathf.Infinity;
            Transform closestFood = null;
            for (int i = 0; i < foodDrops.childCount; i++)
            {
                Transform food = foodDrops.GetChild(i).transform;
                float distance = Vector3.Distance(food.position, transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestFood = food;
                }
            }
            return closestFood;
        }
    }
}