using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Bonkers.BlokControl
{
    public class BlokPool : SerializedMonoBehaviour
    {
        [DictionaryDrawerSettings(KeyLabel = "Individual Blok Pooling Data", ValueLabel = "Blok Type Pool")]
        public Dictionary<IndividualBlokPoolingData, GameObject> BlokPools = new Dictionary<IndividualBlokPoolingData, GameObject>();
    }
}