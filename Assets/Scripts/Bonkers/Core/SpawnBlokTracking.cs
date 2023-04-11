using System.Collections.Generic;
using Bonkers.Events;
using UnityEngine;

namespace Bonkers.Core
{
    public class SpawnBlokTracking : MonoBehaviour
    {
        [SerializeField] private VoidEvent endLevelEvent;
        
        //Hash set so they all have to be unique, just in case
        private HashSet<SpawnBlokReporter> spawnBlokReporters = new HashSet<SpawnBlokReporter>();

        public void AddSpawnBlokToTracker(SpawnBlokReporter spawnBlokReporter)
        {
            spawnBlokReporters.Add(spawnBlokReporter);
        }

        public void RemoveSpawnBlokFromTracker(SpawnBlokReporter spawnBlokReporter)
        {
            spawnBlokReporters.Remove(spawnBlokReporter);
            if (spawnBlokReporters.Count <= 0)
            {
                endLevelEvent.Raise();   
            }
        }
    }
}