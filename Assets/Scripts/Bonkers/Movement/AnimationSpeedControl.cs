using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bonkers.Movement
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(PlayerMovement))]
    public class AnimationSpeedControl : MonoBehaviour
    {
        [SerializeField][Range(1.2f, 5.0f)] float maxSpeedMultiplier = 2.0f;

        Animator animator;
        PlayerMovement mover;

        float baseSpeed;
        float maxBoostEffectSpeed;
        float differenceInSpeed;

        void Awake()
        {
            animator = GetComponent<Animator>();
            mover = GetComponent<PlayerMovement>();
        }

        // Start is called before the first frame update
        void Start()
        {
            baseSpeed = mover.GetMoveSpeed();
            maxBoostEffectSpeed = mover.GetBoostSpeed();
        }

        // Update is called once per frame
        void Update()
        {
            float interpolationRatio = (mover.GetMoveSpeed() - baseSpeed) / maxBoostEffectSpeed;
            float lerpedSpeedMultiplier = Mathf.Lerp(1f, maxSpeedMultiplier, interpolationRatio);
            animator.SetFloat("speed", lerpedSpeedMultiplier);
        }
    }
}
