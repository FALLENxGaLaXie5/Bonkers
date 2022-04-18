using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace Bonkers.BlokControl
{
    public class BlokPool : SerializedMonoBehaviour
    {
        [DictionaryDrawerSettings(KeyLabel = "Individual Blok Pooling Data", ValueLabel = "Blok Type Pool")]
        public Dictionary<IndividualBlokPoolingData, GameObject> BlokPools = new Dictionary<IndividualBlokPoolingData, GameObject>();

        /// <summary>
        /// Grabs a pooled object and sets its new parent
        /// </summary>
        /// <param name="individualBlokPoolingData"></param>
        /// <param name="newParent"></param>
        /// <returns></returns>
        public GameObject GetPooledBlokToSpawn(IndividualBlokPoolingData individualBlokPoolingData, Transform newParent)
        {
            if (BlokPools[individualBlokPoolingData].transform.childCount < 1)
            {
                Debug.LogWarning("There are no objects in the " + individualBlokPoolingData.name + " pool!");
                return null;
            }

            GameObject pooledObject = BlokPools[individualBlokPoolingData].transform.GetChild(0).gameObject;
            pooledObject.transform.SetParent(newParent);
            return pooledObject;
        }

        /// <summary>
        /// This overload will grab the first available random pooled blok to spawn
        /// Returns null if all pools are empty
        /// </summary>
        /// <param name="newParent"></param>
        /// <returns></returns>
        public GameObject GetPooledBlokToSpawn(Transform newParent)
        {
            //Get all available possible individual blok pooling datas, store in list
            Dictionary<IndividualBlokPoolingData, GameObject>.KeyCollection keys = BlokPools.Keys;
            List<IndividualBlokPoolingData> possibleIndividualBlokPoolingDatas = new List<IndividualBlokPoolingData>();
            foreach (var key in keys) possibleIndividualBlokPoolingDatas.Add(key);

            //Gets random possible blok index, and checks if there are bloks in that pool- if not, remove that possible blok from list of bloks to pull from
            //Tries to grab another blok from the next random pool
            int randomBlokIndex = Random.Range(0, possibleIndividualBlokPoolingDatas.Count);
            while (possibleIndividualBlokPoolingDatas.Count > 0 && BlokPools[possibleIndividualBlokPoolingDatas[randomBlokIndex]].transform.childCount < 1)
            {
                possibleIndividualBlokPoolingDatas.RemoveAt(randomBlokIndex);
                randomBlokIndex = Random.Range(0, possibleIndividualBlokPoolingDatas.Count);
            }

            //The random blok index is in the possible blok datas, and that possible blok data exists, and there are bloks in it
            //  return the first possible blok from that pool
            if (possibleIndividualBlokPoolingDatas.Count > 0 && randomBlokIndex < possibleIndividualBlokPoolingDatas.Count)
            {
                IndividualBlokPoolingData blokData = possibleIndividualBlokPoolingDatas[randomBlokIndex];
                GameObject pooledObject = BlokPools[blokData].transform.GetChild(0).gameObject;
                pooledObject.transform.SetParent(newParent);
                return pooledObject;
            }

            //return null if all the blok pools are empty
            Debug.LogWarning("All pools are empty!");
            return null;
        }

        public void AttemptToPoolBlok(GameObject obj)
        {
            BlokDestroyIntoPoolHelper objPoolHelper = obj.GetComponent<BlokDestroyIntoPoolHelper>();
            if (!objPoolHelper)
            {
                Debug.LogError("No pool helper attached!");
                return;
            }
            IndividualBlokPoolingData blokPoolingData = objPoolHelper.PoolingData;
            if (!blokPoolingData)
            {
                Debug.LogError("No pool data set up on pool helper component on the blok!");
                return;
            }
            if (!BlokPools.ContainsKey(blokPoolingData))
            {
                Debug.LogError("Blok pools do not contain a pool for that blok data!");
                return;
            }
            
            obj.transform.parent = BlokPools[blokPoolingData].transform;
        }
    }
}