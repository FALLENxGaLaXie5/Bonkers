using Sirenix.OdinInspector;
using UnityEngine;

namespace Bonkers._2DDestruction
{
    public class BlokFragmentGenerationHelper : MonoBehaviour
    {
        [Button]
        void AssistGenerateFragments()
        {
            Explodable explodableComponent = transform.GetComponent<Explodable>();
            if (!explodableComponent) return;
            explodableComponent.ConfigureFragments();
        }
    }
}