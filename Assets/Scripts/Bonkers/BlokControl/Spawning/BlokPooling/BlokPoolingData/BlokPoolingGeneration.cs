using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Bonkers.BlokControl
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(BlokPool))]
    public class BlokPoolingGeneration : MonoBehaviour
    {
        [DetailedInfoBox("This will assist in creating more of the specified bloks in the blok pools to be spawned in during the game.", "")]
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
                    Breakable breakable = blok.GetComponent<Breakable>();
                    if (!breakable)
                    {
                        Debug.Log("No explodable component attached!");
                        continue;
                    }
                    breakable.ConfigureFragments();
                    blok.SetActive(false);
                }
            }
            
        }
    }
}