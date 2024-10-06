using System.Collections;
using UnityEngine;

namespace Bonkers.ItemDrops
{
    public abstract class PuddleBehavior : MonoBehaviour
    {
        protected abstract IEnumerator WaitToDestroy();

        public void StartWaitingToDestroy() => StartCoroutine(WaitToDestroy());
    }
}