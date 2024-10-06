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

        [FormerlySerializedAs("effectStrengthStrength")] [SerializeField] private float effectStrength = 5;

        [SerializeField] [Range(1, 30)] private float puddleLife = 10f;

        public float EffectStrength => effectStrength;
        public float PuddleLife => puddleLife;


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
            //puddleFadeOutEffect.ExecuteEffect(spriteRenderer);
        }
    }
}