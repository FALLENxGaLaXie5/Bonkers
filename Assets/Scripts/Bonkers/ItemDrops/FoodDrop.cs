using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Bonkers.Drops
{
    [CreateAssetMenu(fileName = "FoodDrop", menuName = "Drops/Make New Droppable Food", order = 0)]
    public class FoodDrop : ScriptableObject, IDroppableObject
    {
        [Title("Assets only")]
        [AssetsOnly][SerializeField] GameObject foodPrefab = null;

        [Title("Class Data")]
        [SerializeField] int priority = 100;

        public void Spawn(Vector3 position)
        {
            //spawn food drop in the drops prefab
            Transform foodDropsTransform = GameObject.FindWithTag("FoodDrops").transform;
            if (foodPrefab && foodDropsTransform) Instantiate(foodPrefab, position, Quaternion.identity).transform.parent = foodDropsTransform;
        }
    }
}
