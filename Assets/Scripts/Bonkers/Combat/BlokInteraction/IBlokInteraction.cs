using UnityEngine;

public interface IBlokInteraction
{
    void BlokHit(Vector3 playerFacingDirection);
    void SetMoving(bool shouldMove, Vector3 playerFacingDirection);
    void BlokBumped(Vector3 playerFacingDirection, Vector3 currentPlayerPosition);
}
