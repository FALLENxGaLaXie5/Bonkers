using Sirenix.OdinInspector;
using UnityEngine;

namespace Bonkers.BlokControl
{
    [CreateAssetMenu(fileName = "Individual Blok Pooling Data", menuName = "Bonkers/Individual Blok Pooling Data", order = 1)]
    public class IndividualBlokPoolingData : BlokPoolingData
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private Color blokEditorMappingColor = Color.white;
        [SerializeField] private int key = 0;
        public GameObject Prefab => prefab;
        public Color BlokEditorMappingColor => blokEditorMappingColor;
        public int Key => key;
    }
}