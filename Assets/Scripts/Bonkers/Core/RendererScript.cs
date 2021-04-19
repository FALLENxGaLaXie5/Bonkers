using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bonkers.Core
{
    [ExecuteInEditMode]
    public class RendererScript : MonoBehaviour
    {
        [SerializeField] string sortingLayer = "TextElement";
        new MeshRenderer renderer;
        // Start is called before the first frame update
        void Start()
        {
            renderer = GetComponent<MeshRenderer>();
            renderer.sortingLayerName = sortingLayer;
        }

        private void Update()
        {
            renderer.sortingLayerName = sortingLayer;
        }
    }
}
