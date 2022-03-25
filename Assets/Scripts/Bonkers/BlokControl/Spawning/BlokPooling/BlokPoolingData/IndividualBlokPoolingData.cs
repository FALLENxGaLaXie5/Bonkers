using UnityEngine;

namespace Bonkers.BlokControl
{
    [CreateAssetMenu(fileName = "Individual Blok Pooling Data", menuName = "Bonkers/Individual Blok Pooling Data", order = 1)]
    public class IndividualBlokPoolingData : BlokPoolingData
    {
        [SerializeField] private GameObject prefab;
        public GameObject Prefab => prefab;
    }
}