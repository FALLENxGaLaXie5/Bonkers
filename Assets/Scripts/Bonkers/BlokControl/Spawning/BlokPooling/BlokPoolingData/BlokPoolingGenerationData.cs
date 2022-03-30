using ScriptableObjectDropdown;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Bonkers.BlokControl
{
    [CreateAssetMenu(fileName = "Blok Pooling Generation Data", menuName = "Bonkers/Blok Pooling Generation Data", order = 1)]
    public class BlokPoolingGenerationData : ScriptableObject
    {
        [SerializeField] private IndividualBlokPoolingData individualBlokPoolingData;
        public IndividualBlokPoolingData IndividualBlokPoolingData => individualBlokPoolingData;
        
        [ShowInInspector, PropertyRange(0, 100)]
        public int numberToGenerate { get; set; }
    }
}