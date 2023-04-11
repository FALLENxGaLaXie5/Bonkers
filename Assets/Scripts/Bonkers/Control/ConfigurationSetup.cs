using Sirenix.OdinInspector;
using UnityEngine;

namespace Bonkers.Control
{
    public class ConfigurationSetup : MonoBehaviour
    {
        [InlineEditor]
        [SerializeField] private PlayerConfigurationSystem playerConfigurationSystem;

        void Awake() => playerConfigurationSystem.ClearPlayerConfigs();

        private void Start() => playerConfigurationSystem.Initialize();
    }
}