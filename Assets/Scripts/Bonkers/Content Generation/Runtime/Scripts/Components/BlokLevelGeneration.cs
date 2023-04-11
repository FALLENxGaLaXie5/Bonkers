#if UNITY_EDITOR

using System.Collections.Generic;
using Bonkers.BlokControl;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bonkers.ContentGeneration
{
    /// <summary>
    /// This will be used by the Content Mapping Editor to generate the level content
    /// based off of the generated content mappings
    /// </summary>
    public class BlokLevelGeneration : SerializedMonoBehaviour
    {
        [DictionaryDrawerSettings(KeyLabel = "Individual Blok Pooling Data", ValueLabel = "Activate Blok Holder")]
        public Dictionary<IndividualBlokPoolingData, GameObject> ActiveBlokHolders = new Dictionary<IndividualBlokPoolingData, GameObject>();

        /// <summary>
        /// Generates the new blok under it's respective parent in the scene passed.
        /// </summary>
        /// <param name="blokMapping"></param>
        /// <param name="scene"></param>
        public void SetupNewBlok(BlokMapping blokMapping, Scene scene)
        {
            GameObject blok = PrefabUtility.InstantiatePrefab(blokMapping.BlokType.Prefab, scene) as GameObject;
            blok.transform.parent = ActiveBlokHolders[blokMapping.BlokType].transform;
            blok.transform.position = new Vector3(blokMapping.Value.x, blokMapping.Value.y, 0);
        }
    }
}
#endif