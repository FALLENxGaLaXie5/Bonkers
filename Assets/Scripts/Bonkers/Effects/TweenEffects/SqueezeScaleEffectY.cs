using DG.Tweening;
using UnityEngine;

namespace  Bonkers.Effects
{
    [CreateAssetMenu(fileName = "New Squeeze Scale Effect Y", menuName = "TweenEffects/Create New Squeeze Scale Effect Y")]
    public class SqueezeScaleEffectY : TweenEffect
    {
        [SerializeField] private float squeezeYAmount = 0.5f;
        [SerializeField] private float elongateXAmount = 0.5f;
        [SerializeField] private float durationIn = 0.04f;
        [SerializeField] private float durationOut = 0.12f;
        public override void ExecuteEffect(Transform transform)
        {
            float originalY = transform.localScale.y;
            float originalX = transform.localScale.x;
            
            Sequence mySequence = DOTween.Sequence();
            mySequence.Append(transform.DOScaleY(transform.localScale.y - squeezeYAmount, durationIn).SetEase(Ease.Linear));
            mySequence.Append(transform.DOScaleY(originalY, durationOut).SetEase(Ease.Linear));
            //this link will automatically kill the tween when the gameobject is destroyed
            mySequence.SetLink(transform.gameObject);

            Sequence myOtherSequence = DOTween.Sequence();
            myOtherSequence.Append(transform.DOScaleX(transform.localScale.x + elongateXAmount, durationIn).SetEase(Ease.Linear));
            myOtherSequence.Append(transform.DOScaleX(originalX, durationOut).SetEase(Ease.Linear));
            //this link will automatically kill the tween when the gameobject is destroyed
            myOtherSequence.SetLink(transform.gameObject);
        }
    }
}