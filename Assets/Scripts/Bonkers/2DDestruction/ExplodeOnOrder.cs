using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Explodable))]
public class ExplodeOnOrder : MonoBehaviour
{
    [SerializeField] float fadeTime = 5f;
    private Explodable _explodable;
    private ExplosionForce ef;

    void Awake()
    {
        _explodable = GetComponent<Explodable>();
        ef = FindObjectOfType<ExplosionForce>();
    }

    public void ExplodeBlok()
    {
        _explodable.explode();
        ef.doExplosion(transform.position);
        _explodable.FadeAndDestroyFragments(fadeTime);
    }
}