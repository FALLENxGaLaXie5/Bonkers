using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Bonkers.Events
{
    public class SpawnBlokReporter : MonoBehaviour
    {
        //field just as an example of how to make public serialized field w/private setting property
        [field:SerializeField] public SpawnBlokReportingEvent spawnBlokInitializingEvent { get; private set; }
        [SerializeField] private SpawnBlokReportingEvent spawnBlokDestroyingEvent;
        
        private void Start() => spawnBlokInitializingEvent.Raise(this);
        
        public void TriggerSpawnBlokDestroyingEvent()
        {
            if (!spawnBlokDestroyingEvent)
            {
                Debug.LogWarning("No spawn blok destroying event set!");
                return;
            }
            spawnBlokDestroyingEvent.Raise(this);
        }
    }
}