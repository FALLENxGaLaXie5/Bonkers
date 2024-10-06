using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Bonkers.ItemDrops
{
    public class TarPuddleBehavior : PuddleBehavior, IEnvironmentEffector
    {
        #region Inspector/Public Variables

        [InlineEditor]
        [SerializeField] PuddleDrop puddle;

        #endregion

        #region Class functions

        protected override IEnumerator WaitToDestroy()
        {
            yield return new WaitForSeconds(puddle.PuddleLife);
            puddle.DestroyPuddle(transform, GetComponent<SpriteRenderer>());
        }

        public ScriptableObject AttemptGetEffector() => puddle;

        #endregion
    }
}