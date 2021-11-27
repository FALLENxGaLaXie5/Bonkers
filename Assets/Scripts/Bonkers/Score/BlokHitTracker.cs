using System;
using UnityEngine;

namespace Bonkers.Score
{
    public class BlokHitTracker : MonoBehaviour
    {
        Action<int> hitCallback;
        
        public void SetHitCallback(Action<int> hitCallback) => this.hitCallback = hitCallback;

        public void Score(int enemyHitScoreValue) => hitCallback?.Invoke(enemyHitScoreValue);
    }
}