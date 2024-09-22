using Animancer;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Bonkers.Animation
{
    public class EnemyAnimation : MonoBehaviour
    {
        [SerializeField]
        private AnimancerComponent _animancerComponent;

        [InlineEditor]
        [SerializeField] private DirectionalAnimationSet _walks;

        private Vector2 Facing { get; set; }

        public void SetFacingDirection(Vector2 facing)
        {
            Facing = facing;
            Play(_walks);
        }

        private void Play(DirectionalAnimationSet animations)
        {
            AnimationClip clip = animations.GetClip(Facing);
            _animancerComponent.Play(clip);
        }
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            // We need to null-check the _walks to prevent GetClip from throwing an exception if it's not assigned yet.
            // But EditModeSampleAnimation will take care of null-checking the AnimationClip and _Animancer.
            if (_walks != null)
                _walks.GetClip(Facing).EditModeSampleAnimation(_animancerComponent);
        }
#endif
    }
}