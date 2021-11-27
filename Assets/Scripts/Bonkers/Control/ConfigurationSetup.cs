using System;
using System.Collections;
using System.Collections.Generic;
using Codice.Client.BaseCommands;
using UnityEngine;

namespace Bonkers.Control
{
    public class ConfigurationSetup : MonoBehaviour
    {
        [SerializeField] private PlayerConfigurationSystem playerConfigurationSystem;

        void Awake() => playerConfigurationSystem.ClearPlayerConfigs();
    }
}