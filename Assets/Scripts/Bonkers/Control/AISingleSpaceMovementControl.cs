using Bonkers.Movement;
using UnityEngine;

namespace Bonkers.Control
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AISingleSpaceMovement))]
    public abstract class AISingleSpaceMovementControl : AIControl
    {
        private AISingleSpaceMovement _singleSpaceMovement;
        protected override void Awake()
        {
            base.Awake();
            _singleSpaceMovement = GetComponent<AISingleSpaceMovement>();

            _singleSpaceMovement.OnChangeDirection += ChangeDirection;
        }

        private void ChangeDirection(Vector2 newDirection) => animation.SetFacingDirection(newDirection);

        protected override void OnDisable() => _singleSpaceMovement.OnChangeDirection -= ChangeDirection;
    }
}