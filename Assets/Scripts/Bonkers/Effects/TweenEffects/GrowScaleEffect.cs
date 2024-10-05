using DG.Tweening;
using UnityEngine;

namespace  Bonkers.Effects
{
    [CreateAssetMenu(fileName = "New Grow Scale Effect", menuName = "TweenEffects/Create New Grow Scale Effect")]
    public class GrowScaleEffect : TweenEffect
    {
        [SerializeField] private float speed = 0.6f;
        [SerializeField] private Ease ease = Ease.OutBounce;
        
        public override void ExecuteEffect(Transform transform)
        {
            if (!transform) return;
            //link will automatically kill the tween when the gameobject is destroyed
            transform.localScale = Vector3.zero;
            transform.DOScale(1f, speed).SetEase(ease);
        }
    }
}