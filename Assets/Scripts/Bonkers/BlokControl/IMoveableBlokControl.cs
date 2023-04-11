using UnityEngine;

public interface IMoveableBlokControl
{
    void SetMoving(bool newIsMoving, Vector3 movementDir);
    void HitEnemy(Transform enemyTransform);
    bool IsMoving();
    float GetCurrentSpeed();
}
