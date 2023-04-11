using Sirenix.OdinInspector;
using UnityEngine;

namespace Bonkers._2DDestruction
{
    public class BlokFragmentGenerationHelper : MonoBehaviour
    {
        [Button]
        void AssistGenerateFragments()
        {
            Breakable breakableComponent = transform.GetComponent<Breakable>();
            if (!breakableComponent) return;
            breakableComponent.ConfigureFragments();
        }
    }
}