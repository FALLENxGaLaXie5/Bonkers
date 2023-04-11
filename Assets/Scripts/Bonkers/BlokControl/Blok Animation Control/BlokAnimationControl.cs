using Sirenix.OdinInspector;
using UnityEngine;

namespace Bonkers.BlokControl
{
    [RequireComponent(typeof(Animator))]
    public abstract class BlokAnimationControl : MonoBehaviour
    {
        [ShowIf("@this.animator == null")]
        [SerializeField] protected Animator animator;
        public abstract void PlayAnimation();
    }
}