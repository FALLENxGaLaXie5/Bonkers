using UnityEngine;

namespace Bonkers.BlokControl
{
    public class SpawnBlokAnimationControl : BlokAnimationControl
    {
        static readonly int spawnTrigger = Animator.StringToHash("spawn");
        
        public override void PlayAnimation()
        {
            animator.SetTrigger(spawnTrigger);
        }
    }
}