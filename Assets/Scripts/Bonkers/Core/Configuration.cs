using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Bonkers.Core
{
    public class Configuration : MonoBehaviour
    {
        [Button]
        void ConfigureExplodableBloks()
        {
            var explodableObjects = FindObjectsOfType<Explodable>();
            foreach (var explodable in explodableObjects)
            {
                explodable.ConfigureFragments();
            }
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}