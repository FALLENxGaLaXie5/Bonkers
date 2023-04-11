using UnityEngine;

namespace Bonkers.BlokControl
{
    public class BombBlokExplosionObjectControl : MonoBehaviour
    {
        [SerializeField] private Transform parentController;

        private bool exploded = false;

        public void Explode() => exploded = true;
        public bool Exploded => exploded;

        public void ResetExplosionObjectController()
        {
            transform.parent = parentController;
            transform.localPosition = new Vector3(0, 0, 0);
            transform.gameObject.SetActive(false);
            exploded = false;
        }
    }
}