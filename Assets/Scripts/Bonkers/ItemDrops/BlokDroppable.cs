using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bonkers.Drops
{
    public class BlokDroppable : MonoBehaviour
    {
        //can be food, powerups, etc. Scriptable objects
        [SerializeField] FoodDrop[] foodDrops;
        [SerializeField] Powerup[] powerupDrops;

        List<IDroppableObject> drops = new List<IDroppableObject>();

        public void Start()
        {
            GenerateDropsList();
        }

        public void SpawnDrop()
        {
            if (drops.Count <= 0) return;
            //for now, will just spawn a random drop from the ones we have available (could be food, powerup, etc)
            int rand = Random.Range(0, drops.Count);
            drops[rand].Spawn(transform.position);
        }

        void GenerateDropsList()
        {
            foreach (FoodDrop foodDrop in foodDrops)
            {
                drops.Add(foodDrop);
            }
            foreach (Powerup powerupDrop in powerupDrops)
            {
                drops.Add(powerupDrop);
            }
        }
    }

}