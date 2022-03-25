using Sirenix.OdinInspector;
using System.Collections.Generic;
using ScriptableObjectDropdown;
using UnityEngine;
using UnityEngine.UI;

namespace Bonkers.BlokControl
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(BlokPool))]
    public class BlokPoolingGeneration : MonoBehaviour
    {
        [SerializeField] private BlokPoolingGenerationData blokPoolingGenerationData;

        private BlokPool blokPool;

        void OnEnable() => blokPool = GetComponent<BlokPool>();

        [Button("Pool Data")]
        public void PoolData()
        {
            if (!blokPool) blokPool = GetComponent<BlokPool>();
            if (!blokPoolingGenerationData)
            {
                Debug.LogError("No blok pool generation data referenced!");
                return;
            }

            for (int i = 0; i < blokPoolingGenerationData.numberToGenerate; i++)
            {
                //Intantiate new prefab and set it's parent to it's appropriate pool
                GameObject blok = Instantiate(blokPoolingGenerationData.IndividualBlokPoolingData.Prefab);
                blok.transform.parent = blokPool.BlokPools[blokPoolingGenerationData.IndividualBlokPoolingData].transform;
            }
        }
    }
}