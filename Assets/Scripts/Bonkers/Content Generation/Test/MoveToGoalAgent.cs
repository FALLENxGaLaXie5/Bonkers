using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace Bonkers.Content_Generation
{
    public class MoveToGoalAgent : Agent
    {
        [SerializeField] private Transform targetTransform;
        [SerializeField] private Material winMaterial;
        [SerializeField] private Material loseMaterial;
        [SerializeField] private MeshRenderer floorMeshRenderer;

        public override void OnEpisodeBegin()
        {
            //transform.localPosition = Vector3.zero;
            transform.localPosition = new Vector3(Random.Range(-1.4f, 1f), 0,Random.Range(0f, 0.9f));
            targetTransform.localPosition = new Vector3(Random.Range(-1.4f, 1f), 0, -1.8f);
        }
        
        public override void CollectObservations(VectorSensor sensor)
        {
            //Configured as 6 observations, 1 for each float in 2 Vector3s
            sensor.AddObservation(transform.localPosition);
            sensor.AddObservation(targetTransform.localPosition);
        }
        
        public override void OnActionReceived(ActionBuffers actions)
        {
            float moveX = actions.ContinuousActions[0];
            float moveZ = actions.ContinuousActions[1];

            float moveSpeed = 1f;
            transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
            continuousActions[0] = Input.GetAxisRaw("Horizontal");
            continuousActions[1] = Input.GetAxisRaw("Vertical");
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Goal goal))
            {
                SetReward(1f);
                floorMeshRenderer.material = winMaterial;
                EndEpisode();
            }
            if (other.TryGetComponent(out Wall wall))
            {
                SetReward(-1f);
                floorMeshRenderer.material = loseMaterial;
                EndEpisode();
            }
        }
    }
}