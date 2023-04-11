using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyCombat
{
    void HitPlayer(Transform playerTransform);
    void DisableCombat();
}
