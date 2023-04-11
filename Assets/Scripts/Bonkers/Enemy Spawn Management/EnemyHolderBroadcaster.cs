using System;
using Bonkers.Events;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Bonkers.EnemySpawnManagement
{
    public class EnemyHolderBroadcaster : MonoBehaviour
    {
        [ShowIf("@this.enemyHolders == null")]
        [SerializeField] private GameObject enemyHolders;

        [SerializeField] private LegacyGameEventWithGameObject enemyHoldersObjectSetupEvent;

        private void Start()
        {
            enemyHoldersObjectSetupEvent.Raise(enemyHolders);
        }
    }
}