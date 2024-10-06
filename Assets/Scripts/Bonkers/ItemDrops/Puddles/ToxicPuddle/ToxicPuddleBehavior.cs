using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Bonkers.ItemDrops
{
    public class ToxicPuddleBehavior : PuddleBehavior, IEnvironmentEffector
    {
        #region Inspector/Public Variables

        [FormerlySerializedAs("puddle")]
        [InlineEditor]
        [SerializeField] PuddleDrop puddleDrop;
        [SerializeField] float chanceOfPuddleSpawn = 20f;

        [SerializeField] private UnityEvent<Vector3> AttemptSpawnEvent;
        #endregion

        #region Class functions

        protected override IEnumerator WaitToDestroy()
        {
            yield return new WaitForSeconds(puddleDrop.PuddleLife);
            //try to spawn new toxicccc life!
            AttemptSpawnNewToxicSlimo();
            puddleDrop.DestroyPuddle(transform, GetComponent<SpriteRenderer>());
        }

        void AttemptSpawnNewToxicSlimo()
        {
            //if random generator does not favor spawning one, return
            if (Random.Range(0, 100) >= chanceOfPuddleSpawn) return;

            AttemptSpawnEvent?.Invoke(transform.position);
        }

        public ScriptableObject AttemptGetEffector() => puddleDrop;

        #endregion
    }
}

