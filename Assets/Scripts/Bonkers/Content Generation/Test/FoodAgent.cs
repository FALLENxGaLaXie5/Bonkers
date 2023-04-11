using System;
using Sirenix.OdinInspector;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Bonkers.Content_Generation
{
    public class FoodAgent : Agent
    {
        public event EventHandler OnAteFood;
        public event EventHandler OnEpisodeBeginEvent;

        [SerializeField] private FoodSpawner foodSpawner;
        [SerializeField] private FoodButton foodButton;

        private Rigidbody agentRigidBody;

        private void Awake()
        {
            agentRigidBody = GetComponent<Rigidbody>();
        }

        public override void OnEpisodeBegin()
        {
            transform.localPosition = new Vector3(UnityEngine.Random.Range(-2.5f, +2.5f), 0,
                UnityEngine.Random.Range(-2.0f, 5.0f));
            OnEpisodeBeginEvent?.Invoke(this, EventArgs.Empty);
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            sensor.AddObservation(foodButton.CanUseButton() ? 1 : 0);

            Vector3 dirToFoodButton = (foodButton.transform.localPosition - transform.localPosition).normalized;
            sensor.AddObservation(dirToFoodButton.x);
            sensor.AddObservation(dirToFoodButton.z);
            
            sensor.AddObservation(foodSpawner.HasFoodSpawned() ? 1 : 0);

            if (foodSpawner.HasFoodSpawned())
            {
                Vector3 dirToFood = (foodSpawner.GetLastFoodTransform().localPosition - transform.localPosition)
                    .normalized;
                sensor.AddObservation(dirToFood.x);
                sensor.AddObservation(dirToFood.z);
            }
            else
            {
                sensor.AddObservation(0f);
                sensor.AddObservation(0f);
            }
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            int moveX = actions.DiscreteActions[0]; // 0 = Don't Move; 1 = Left; 2 = Right
            int moveZ = actions.DiscreteActions[1]; // 0 = Don't Move; 1 = Back; 2 = Forward

            Vector3 addForce = new Vector3(0, 0, 0);

            switch (moveX)
            {
                case 0: addForce.x = 0f;
                    break;
                case 1: addForce.x = -1f;
                    break;
                case 2: addForce.x = +1f;
                    break;
            }

            switch (moveZ)
            {
                case 0: addForce.z = 0f;
                    break;
                case 1: addForce.z = -1f;
                    break;
                case 2: addForce.z = +1f;
                    break;
            }

            float moveSpeed = 5f;
            agentRigidBody.velocity = addForce * moveSpeed + new Vector3(0, agentRigidBody.velocity.y, 0);

            bool isUseButtonDown = actions.DiscreteActions[2] == 1;
            if (isUseButtonDown)
            {
                // Use Action
                Collider[] colliderArray = Physics.OverlapBox(transform.position, Vector3.one * 0.5f);
                foreach (Collider collider in colliderArray)
                {
                    if (collider.TryGetComponent(out FoodButton foodButton))
                    {
                        if (foodButton.CanUseButton())
                        {
                            foodButton.UseButton();
                            AddReward(1f);
                        }
                    }
                }
            }
            
            AddReward(-1f / MaxStep);
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            ActionSegment<int> discreteActions = actionsOut.DiscreteActions;

            switch (Mathf.RoundToInt(Input.GetAxisRaw("Horizontal")))
            {
                case -1: discreteActions[0] = 1;
                    break;
                case 0: discreteActions[0] = 0; 
                    break;
                case +1: discreteActions[0] = 2;
                    break;
            }

            switch (Mathf.RoundToInt(Input.GetAxisRaw("Vertical")))
            {
                case -1: discreteActions[1] = 1;
                    break;
                case 0: discreteActions[1] = 0; 
                    break;
                case +1: discreteActions[1] = 2;
                    break;
            }

            discreteActions[2] = Input.GetKey(KeyCode.E) ? 1 : 0; // Use Action
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.TryGetComponent(out Food food))
            {
                AddReward(1f);
                Destroy(food.gameObject);
                OnAteFood?.Invoke(this, EventArgs.Empty);
                
                EndEpisode();
            }

            /*
            if (collision.gameObject.TryGetComponent(out Wall wall))
            {
                EndEpisode();
            }
            */
        }
    }

    internal class Food : MonoBehaviour
    {
    }

    internal class FoodButton : MonoBehaviour
    {
        public bool CanUseButton()
        {
            return false;
        }

        public void UseButton()
        {
            
        }
    }

    internal class FoodSpawner
    {
        public bool HasFoodSpawned()
        {
            return false;
        }

        public Transform GetLastFoodTransform()
        {
            return null;
        }
    }
}