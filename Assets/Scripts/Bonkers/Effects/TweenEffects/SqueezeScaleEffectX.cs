using DG.Tweening;
using UnityEngine;

namespace  Bonkers.Effects
{
    [CreateAssetMenu(fileName = "New Squeeze Scale Effect X", menuName = "TweenEffects/Create New Squeeze Scale Effect X")]
    public class SqueezeScaleEffectX : TweenEffect
    {
        [SerializeField] private float squeezeXAmount = 0.5f;
        [SerializeField] private float elongateYAmount = 0.5f;
        [SerializeField] private float durationIn = 0.04f;
        [SerializeField] private float durationOut = 0.12f;
        public override void ExecuteEffect(Transform transform)
        {
            float originalX = transform.localScale.x;
            float originalY = transform.localScale.y;
            Sequence mySequence = DOTween.Sequence();
            mySequence.Append(transform.DOScaleX(transform.localScale.x - squeezeXAmount, durationIn).SetEase(Ease.Linear));
            mySequence.Append(transform.DOScaleX(originalX, durationOut).SetEase(Ease.Linear));
            //this link will automatically kill the tween when the gameobject is destroyed
            mySequence.SetLink(transform.gameObject);

            Sequence myOtherSequence = DOTween.Sequence();
            myOtherSequence.Append(transform.DOScaleY(transform.localScale.y + elongateYAmount, durationIn).SetEase(Ease.Linear));
            myOtherSequence.Append(transform.DOScaleY(originalY, durationOut).SetEase(Ease.Linear));
            //this link will automatically kill the tween when the gameobject is destroyed
            myOtherSequence.SetLink(transform.gameObject);
        }
    }
}