using UnityEngine;
using Sirenix.OdinInspector;

namespace Bonkers.Core
{
    public class Configuration : MonoBehaviour
    {
        [DetailedInfoBox("This will assist with configuring all bloks in the game. It will find all Breakable components\n and " +
                         "delete any fragments existing under them, then add new fragments.", "")]
        /// <summary>
        /// This will delete any fragments then generate new ones. Still need to apply new materials
        /// </summary>
        [Button]
        public void ConfigureExplodableBloks()
        {
            var explodableObjects = FindObjectsOfType<Breakable>(true);
            foreach (Breakable explodable in explodableObjects)
            {
                explodable.ConfigureFragments();
            }
        }
    }
}