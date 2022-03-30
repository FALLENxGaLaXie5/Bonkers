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
        [SerializeField] private List<BlokPoolingGenerationData> blokPoolingGenerationDatas;
        private BlokPool blokPool;

        void OnEnable() => blokPool = GetComponent<BlokPool>();

        [Button("Pool Individual Blocks")]
        public void PoolData()
        {
            if (!blokPool) blokPool = GetComponent<BlokPool>();
            if (blokPoolingGenerationDatas == null || blokPoolingGenerationDatas.Count < 1)
            {
                Debug.LogError("Nothing in the block pool generation datas!");
                return;
            }

            foreach (BlokPoolingGenerationData blokPoolGenerationData in blokPoolingGenerationDatas)
            {
                for (int i = 0; i < blokPoolGenerationData.numberToGenerate; i++)
                {
                    //Intantiate new prefab and set it's parent to it's appropriate pool
                    GameObject blok = Instantiate(blokPoolGenerationData.IndividualBlokPoolingData.Prefab);
                    blok.transform.parent = blokPool.BlokPools[blokPoolGenerationData.IndividualBlokPoolingData].transform;
                    Explodable explodable = blok.GetComponent<Explodable>();
                    if (!explodable)
                    {
                        Debug.Log("No explodable component attached!");
                        continue;
                    }
                    explodable.ConfigureFragments();
                    blok.SetActive(false);
                }
            }
            
        }
    }
}