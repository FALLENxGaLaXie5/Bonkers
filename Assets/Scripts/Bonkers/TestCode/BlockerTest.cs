using UnityEngine;
using System.Collections;
using Pathfinding;
using System;

public class BlockerTest : MonoBehaviour
{
    [SerializeField] float updateTime = 0.2f;
    SingleNodeBlocker blocker;
    public void Start()
    {
        blocker = GetComponent<SingleNodeBlocker>();
        StartCoroutine(UpdateBlockedPosition());
    }

    IEnumerator UpdateBlockedPosition()
    {
        while (true)
        {
            BlockAtCurrentPosition();
            yield return new WaitForSeconds(updateTime);
        }
    }

    void BlockAtCurrentPosition()
    {
        blocker.BlockAtCurrentPosition();
    }
}