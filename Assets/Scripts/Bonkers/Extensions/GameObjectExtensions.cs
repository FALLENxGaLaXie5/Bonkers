using UnityEngine;
using Bonkers._2DDestruction;

namespace Bonkers.Extensions
{
    public static class GameObjectExtensions
    {
        public static void StoreFragmentData(this GameObject fragment, Transform parent)
        {
            if (fragment.GetComponent<FragmentDataStorage>()) return;
            //store original local position and fragment parent so it can be put back together later
            FragmentDataStorage fragmentDataStorage = fragment.AddComponent<FragmentDataStorage>();
            fragmentDataStorage.StoreOriginalParent(parent);
            fragmentDataStorage.StoreOriginalLocalPosition(fragment.transform.localPosition);
        }
    }   
}