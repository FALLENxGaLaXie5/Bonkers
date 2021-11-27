using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Bonkers.Control;

namespace Bonkers.Control
{
    public class InitializeLevel : MonoBehaviour
    {
        [SerializeField] private PlayerConfigurationSystem playerConfigurationManger;
        [SerializeField] Transform[] playerSpawns;
        [SerializeField] GameObject playerPrefab;

        void Start()
        {
            var playerConfigs = playerConfigurationManger.GetPlayerConfigs().ToArray();
            for (int i = 0; i < playerConfigs.Length; i++)
            {
                var player = Instantiate(playerPrefab, playerSpawns[i].position, playerSpawns[i].rotation, gameObject.transform);
                player.GetComponent<PlayerInputHandler>().InitializePlayer(playerConfigs[i], i+1);
            }
        }
    }
}