using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBlokControl
{
    void SetMoving(bool newIsMoving, Vector3 movementDir);
    float GetCurrentSpeed();
    bool IsMoving();
    void PlaySound();
    void HitEnemy(Transform enemyTransform);
}
