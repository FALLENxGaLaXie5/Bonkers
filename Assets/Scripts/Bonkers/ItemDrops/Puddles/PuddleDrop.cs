using Bonkers.Effects;
using UnityEngine;

namespace Bonkers.ItemDrops
{
    [CreateAssetMenu(fileName = "PuddleDrop", menuName = "Drops/Make New Droppable Puddle", order = 1)]
    public class PuddleDrop : ScriptableObject
    {
        [SerializeField] private GameObject puddlePrefab;
        [SerializeField] private TweenEffect puddleGrowEffect;
        [SerializeField] private float effect = 5;
        [SerializeField] [Range(1, 30)] private float puddleLife = 10f;

        public void Spawn(Vector3 position)
        {
            if (puddlePrefab) Instantiate(puddlePrefab, position, Quaternion.identity);
        }

        public float GetEffect() => effect;

        public float GetLife() => puddleLife;
    }
}