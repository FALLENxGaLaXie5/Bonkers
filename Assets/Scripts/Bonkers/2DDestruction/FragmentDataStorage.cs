using System.Runtime.CompilerServices;
using UnityEngine;

namespace Bonkers._2DDestruction
{
    public class FragmentDataStorage : MonoBehaviour
    {
        public Transform parent { get; private set; }
        public Vector3 localPosition { get; private set; }

        private bool fragmentDataStored = false;
        public bool FragmentDataStored => fragmentDataStored;
        public void SetFragmentDataAsStored() => fragmentDataStored = true;

        public void StoreOriginalParent(Transform parent) => this.parent = parent;
        public void StoreOriginalLocalPosition(Vector3 localPosition) => this.localPosition = localPosition;
    }
}