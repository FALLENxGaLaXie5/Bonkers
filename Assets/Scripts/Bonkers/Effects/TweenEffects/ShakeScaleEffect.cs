using System;
using DG.Tweening;
using UnityEngine;

namespace  Bonkers.Effects
{
    [CreateAssetMenu(fileName = "New Shake Scale Effect", menuName = "TweenEffects/Create New Shake Scale Effect")]
    public class ShakeScaleEffect : TweenEffect<Transform>
    {
        [SerializeField] float duration = 0.5f;
        [SerializeField] Vector3 strength = new Vector3(20f, 20f, 20f);
        [SerializeField] int vibrato = 30;
        [SerializeField] float randomness = 90f;
        [SerializeField] bool fadeOut = true;
        
        public override void ExecuteEffect(Transform transform, Action action = null)
        {
            if (!transform) return;
            //link will automatically kill the tween when the gameobject is destroyed
            transform.DOShakeScale(duration,  strength, vibrato, randomness, fadeOut).SetLink(transform.gameObject).OnComplete(() =>
            {
                action?.Invoke();
            });;;
        }
    }
}

