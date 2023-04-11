using System.Collections.Generic;
using Bonkers.Events;
using UnityEngine;

namespace Bonkers.BlokControl
{
    [RequireComponent(typeof(SpawnBlokReporter))]
    public class SpawnBlokHealth : BlokHealth
    {
        [SerializeField] private SpawnBlokReporter spawnBlokReporter;
        
        /// <summary>
        /// Adding on a trigger for destroying this spawn blok since the game loop needs to track
        /// spawn bloks being destroyed specifically
        /// </summary>
        /// <param name="waitTime"></param>
        /// <param name="fragmentComponents"></param>
        public override void DestroyBlok(float waitTime, List<AnimateFragmentOut> fragmentComponents)
        {
            base.DestroyBlok(waitTime, fragmentComponents);
            spawnBlokReporter.TriggerSpawnBlokDestroyingEvent();
        }
    }
}