using UnityEngine;

namespace Bonkers.Extensions
{
    public static class TransformExtensions
    {
        public static Transform Clear(this Transform transform)
        {
            foreach (Transform child in transform)
            {
                GameObject.Destroy(child.gameObject);
            }

            return transform;
        }

        public static Transform ClearImmediate(this Transform transform)
        {
            foreach (Transform child in transform)
            {
                GameObject.DestroyImmediate(child.gameObject);
            }

            return transform;
        }
        
        public static Transform SetChildrenInactiveImmediate(this Transform transform)
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }

            return transform;
        }
    }
}