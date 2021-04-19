using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bonkers.Combat
{
    [RequireComponent(typeof(Explodable))]
    public class ExplodeOnOrder : MonoBehaviour
    {
        [SerializeField] float fadeTime = 5f;
        private Explodable _explodable;
        private ExplosionForce ef;
        // Start is called before the first frame update
        void Start()
        {
            _explodable = GetComponent<Explodable>();
            ef = GameObject.FindObjectOfType<ExplosionForce>();
        }

        public void ExplodeBlok()
        {
            _explodable.explode();
            ef.doExplosion(transform.position);
            _explodable.FadeAndDestroyFragments(fadeTime);
        }
    }
}
