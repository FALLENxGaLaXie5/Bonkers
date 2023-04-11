using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImmovableBlokControl : MonoBehaviour, IMoveableBlokControl
{
    [SerializeField] float moveSpeed = 0f;
    bool isMoving = false;
    Vector3 moveDir = Vector3.zero;

    public void HitEnemy(Transform enemyTransform)
    {
        
    }

    public bool IsMoving()
    {
        return this.isMoving;
    }

    public void PlaySound()
    {
        
    }

    public void SetMoving(bool newIsMoving, Vector3 movementDir)
    {
        this.isMoving = newIsMoving;
        this.moveDir = movementDir;
    }

    public float GetCurrentSpeed()
    {
        return this.moveSpeed;
    }
}
