using Sirenix.OdinInspector;
using UnityEngine;

namespace Bonkers._2DDestruction
{
    public class BlokFragmentMaterialModificationHelper : MonoBehaviour
    {
        [DetailedInfoBox("Modifies all Blok Fragment Materials to the lit URP Shader.\nFor use after Blok Fragments have been generated.", "")]
        [SerializeField] Shader fragmentShader;
        
        /// <summary>
        /// Sets up all fragment materials to the preset shader given (specific fix for an issue to make it all compatible with URP)
        /// </summary>
        [Button]
        public void AssistMatchFragmentMaterials()
        {
            AnimateFragmentOut[] fragments = GetComponentsInChildren<AnimateFragmentOut>(true);
            foreach (var fragmentComponent in fragments)
            {
                MeshRenderer renderer = fragmentComponent.transform.GetComponent<MeshRenderer>();
                renderer.sharedMaterial.shader = fragmentShader;
            }
        }
    }
}