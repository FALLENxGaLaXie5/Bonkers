using System.Collections.Generic;
using Bonkers.Effects;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Bonkers.ItemDrops
{
    [CreateAssetMenu(fileName = "PuddleDrop", menuName = "Drops/Make New Droppable Puddle", order = 1)]
    public class PuddleDrop : ScriptableObject
    {
        [SerializeField] private GameObject puddlePrefab;
        [InlineEditor][SerializeField] private TweenEffect<Transform> puddleGrowEffect;
        [InlineEditor][SerializeField] private TweenEffect<Transform> puddleShrinkEffect;
        [InlineEditor][SerializeField] private TweenEffect<SpriteRenderer> puddleFadeInEffect;
        [InlineEditor][SerializeField] private TweenEffect<SpriteRenderer> puddleFadeOutEffect;

        [Header("Speed Effect Settings")]
        [SerializeField] [Range(0.1f, 3f)] private float effectTransitionDuration = 0.5f;
        [FormerlySerializedAs("effectStrengthStrength")] [SerializeField] private float effectStrength = 3;
        [SerializeField] [Range(1, 30)] private float puddleLife = 10f;

        [Header("Puddle Caused Player Visual Effects")]
        [InlineEditor][SerializeField] private List<TweenEffect<SpriteRenderer>> playerVisualEffects = new List<TweenEffect<SpriteRenderer>>();

        public float EffectStrength => effectStrength;
        public float EffectTransitionDuration => effectTransitionDuration;
        public float PuddleLife => puddleLife;
        public List<TweenEffect<SpriteRenderer>> PlayerVisualEffects => playerVisualEffects;

        public void Spawn(Vector3 position)
        {
            GameObject puddle = Instantiate(puddlePrefab, position, Quaternion.identity);
            PuddleBehavior puddleBehavior = puddle.GetComponent<PuddleBehavior>();
            puddleGrowEffect.ExecuteEffect(puddle.transform);

            //This will start the puddle's life counter after it's fully faded in
            puddleFadeInEffect.ExecuteEffect(puddle.GetComponent<SpriteRenderer>(), puddleBehavior.StartWaitingToDestroy);
        }

        public void DestroyPuddle(Transform transform, SpriteRenderer spriteRenderer)
        {
            puddleShrinkEffect.ExecuteEffect(transform, () => { Destroy(transform.gameObject); });
            puddleFadeOutEffect.ExecuteEffect(spriteRenderer);
        }

        public void ApplyPlayerVisualEffects(SpriteRenderer playerSpriteRenderer)
        {
            foreach (TweenEffect<SpriteRenderer> effect in playerVisualEffects)
            {
                effect.ExecuteEffect(playerSpriteRenderer);
            }
        }

        public void StopPlayerVisualEffects(SpriteRenderer playerSpriteRenderer)
        {
            foreach (TweenEffect<SpriteRenderer> effect in playerVisualEffects)
            {
                effect.StopEffect(playerSpriteRenderer);
            }
        }
    }
}