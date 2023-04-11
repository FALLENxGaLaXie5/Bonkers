using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bonkers.Combat;
using Bonkers.Movement;

namespace Bonkers.Control
{
    public class AIControlPathfinder : AIControl
    {
        protected AIPathfinderMovement moverPathfinder;

        protected override void Start()
        {
            moverPathfinder = aiMovement as AIPathfinderMovement;
            moverPathfinder.ChooseNewPatrolTarget();
        }
    }
}
