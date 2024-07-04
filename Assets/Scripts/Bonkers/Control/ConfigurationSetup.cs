using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Bonkers.Control
{
    public class ConfigurationSetup : MonoBehaviour
    {
        [InlineEditor]
        [SerializeField] private PlayerConfigurationSystem playerConfigurationSystem;
        
        private void OnApplicationQuit() => playerConfigurationSystem.ClearPlayerConfigs();

        private void Start() => playerConfigurationSystem.Initialize();
    }
}