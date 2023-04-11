using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bonkers.Drops
{
    [CreateAssetMenu(fileName = "PuddleDrop", menuName = "Drops/Make New Droppable Puddle", order = 1)]
    public class PuddleDrop : ScriptableObject
    {
        [SerializeField] GameObject puddlePrefab = null;
        [SerializeField] float effect = 5;
        [SerializeField] [Range(1, 30)] float puddleLife = 10f;

        public void Spawn(Vector3 position)
        {
            if (puddlePrefab) Instantiate(puddlePrefab, position, Quaternion.identity);
        }

        public float GetEffect()
        {
            return this.effect;
        }

        public float GetLife()
        {
            return this.puddleLife;
        }
    }
}