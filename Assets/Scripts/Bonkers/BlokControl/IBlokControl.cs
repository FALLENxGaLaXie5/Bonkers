using UnityEngine;

public interface IBlokControl
{
    void SetMoving(bool newIsMoving, Vector3 movementDir);
    //void PlaySound();
    void HitEnemy(Transform enemyTransform);
    bool IsMoving();
    float GetCurrentSpeed();
}
