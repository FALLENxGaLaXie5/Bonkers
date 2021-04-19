using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bonkers.Drops
{
    [CreateAssetMenu(fileName = "Powerup", menuName = "Drops/Make New Powerup", order = 2)]
    public class Powerup : ScriptableObject, IDroppableObject
    {
        [SerializeField] string type = "";
        [SerializeField] GameObject powerupPrefab = null;
        [SerializeField] float life = 5f;
        [SerializeField] [Range(0.01f, 0.1f)] float modifier = 0.03f;
        [SerializeField] [ColorUsage(true, true)] Color powerupColor;

        public void Spawn(Vector3 position)
        {
            Transform powerupDropsTransform = GameObject.FindWithTag("Drops").transform;
            if (powerupPrefab) Instantiate(powerupPrefab, position, Quaternion.identity).transform.parent = powerupDropsTransform;
        }        

        public float GetLife()
        {
            return life;
        }

        public string GetName()
        {
            return this.type;
        }

        public Color GetColor()
        {
            return this.powerupColor;
        }

        public float GetModifier()
        {
            return modifier;
        }
    }
}