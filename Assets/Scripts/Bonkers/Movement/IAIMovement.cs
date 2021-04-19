using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System;

namespace Bonkers.Movement
{
    public interface IAIMovement
    {
        void Patrol();
        void BackTrackPath(Vector3 currentFacingDir);
        void DisableMovement();
    }

}
