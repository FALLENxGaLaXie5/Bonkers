﻿using System.Collections.Generic;
using Bonkers._2DDestruction;
using Sirenix.OdinInspector;
using UnityEngine;

public class Explodable : MonoBehaviour
{
    public System.Action<List<GameObject>> OnFragmentsGenerated;

    public bool allowRuntimeFragmentation = false;
    public int extraPoints = 0;
    public int subshatterSteps = 0;

    public string fragmentLayer = "Default";
    public string sortingLayerName = "Default";
    public int orderInLayer = 0;
    private float dragCoefficient = 1f;

    public enum ShatterType
    {
        Triangle,
        Voronoi
    };
    public ShatterType shatterType;
    public List<GameObject> fragments = new List<GameObject>();
    private List<List<Vector2>> polygons = new List<List<Vector2>>();

    void Start()
    {
        var animatedComps = GetComponentsInChildren<AnimateFragmentOut>(true);
        foreach (var comp in animatedComps)
        {
            fragments.Add(comp.gameObject);
        }
        if (allowRuntimeFragmentation)
        {
            ModifyFragments();
        }        
    }

    public void ConfigureFragments()
    {
        fragmentInEditor();
        ModifyFragments();
    }

    void ModifyFragments()
    {
        if (fragments.Count > 0)
        {
            foreach (GameObject fragment in fragments)
            {
                fragment.transform.GetComponent<Rigidbody2D>().gravityScale = 0f;
                fragment.transform.GetComponent<Rigidbody2D>().drag = dragCoefficient;
                AnimateFragmentOut animateFragmentOut = fragment.GetComponent<AnimateFragmentOut>();
                if (!animateFragmentOut)
                {
                    animateFragmentOut = fragment.AddComponent<AnimateFragmentOut>();
                }
                animateFragmentOut.AssignRenderer();
                StoreFragmentData(fragment);
            }
        }        
    }

    private void StoreFragmentData(GameObject fragment)
    {
        if (fragment.GetComponent<FragmentDataStorage>()) return;
        //store original local position and fragment parent so it can be put back together later
        FragmentDataStorage fragmentDataStorage = fragment.AddComponent<FragmentDataStorage>();
        fragmentDataStorage.StoreOriginalParent(transform);
        fragmentDataStorage.StoreOriginalLocalPosition(fragment.transform.localPosition);
    }

    public void FadeAndDestroyFragments(float fadeTime)
    {
        foreach (GameObject fragment in fragments)
        {
            fragment.GetComponent<AnimateFragmentOut>().Fade(fadeTime);
        }
    }

    /// <summary>
    /// Creates fragments if necessary and destroys original gameobject
    /// </summary>
    public void explode()
    {
        if(fragments.Count <= 0)
        {
            Debug.LogWarning("No fragments generated; must generate before runtime!");
            return;
        }

        foreach (GameObject frag in fragments)
        {
            frag.transform.parent = null;
            frag.SetActive(true);
        }
        
        //if fragments exist, throw object outside of play area so it can finish any processing
        if (fragments.Count > 0) transform.position = new Vector3(-100, -100, 0);
    }

    /// <summary>
    /// Creates fragments and then disables them
    /// </summary>
    public void fragmentInEditor()
    {
        if (fragments.Count > 0)
        {
            deleteFragments();
        }
        generateFragments();
        setPolygonsForDrawing();
        foreach (GameObject frag in fragments)
        {
            frag.transform.parent = transform;
            frag.SetActive(false);
        }
    }

    public void deleteFragments()
    {
        foreach (GameObject frag in fragments)
        {
            if (Application.isEditor)
            {
                DestroyImmediate(frag);
            }
            else
            {
                Destroy(frag);
            }
        }
        fragments.Clear();
        polygons.Clear();
    }

    /// <summary>
    /// Turns Gameobject into multiple fragments
    /// </summary>
    void generateFragments()
    {
        fragments = new List<GameObject>();
        switch (shatterType)
        {
            case ShatterType.Triangle:
                fragments = SpriteExploder.GenerateTriangularPieces(gameObject, extraPoints, subshatterSteps);
                break;
            case ShatterType.Voronoi:
                fragments = SpriteExploder.GenerateVoronoiPieces(gameObject, extraPoints, subshatterSteps);
                break;
            default:
                Debug.Log("invalid choice");
                break;
        }
        //sets additional aspects of the fragments
        foreach (GameObject p in fragments)
        {
            if (p != null)
            {
                p.layer = LayerMask.NameToLayer(fragmentLayer);
                p.GetComponent<Renderer>().sortingLayerName = sortingLayerName;
                p.GetComponent<Renderer>().sortingOrder = orderInLayer;
            }
        }

        foreach (ExplodableAddon addon in GetComponents<ExplodableAddon>())
        {
            if (addon.enabled)
            {
                addon.OnFragmentsGenerated(fragments);
            }
        }
    }
    void setPolygonsForDrawing()
    {
        polygons.Clear();
        List<Vector2> polygon;

        foreach (GameObject frag in fragments)
        {
            polygon = new List<Vector2>();
            foreach (Vector2 point in frag.GetComponent<PolygonCollider2D>().points)
            {
                Vector2 offset = rotateAroundPivot((Vector2)frag.transform.position, (Vector2)transform.position, Quaternion.Inverse(transform.rotation)) - (Vector2)transform.position;
                offset.x /= transform.localScale.x;
                offset.y /= transform.localScale.y;
                polygon.Add(point + offset);
            }
            polygons.Add(polygon);
        }
    }
    private Vector2 rotateAroundPivot(Vector2 point, Vector2 pivot, Quaternion angle)
    {
        Vector2 dir = point - pivot;
        dir = angle * dir;
        point = dir + pivot;
        return point;
    }

    void OnDrawGizmos()
    {
        if (Application.isEditor)
        {
            if (polygons.Count == 0 && fragments.Count != 0)
            {
                setPolygonsForDrawing();
            }

            Gizmos.color = Color.blue;
            Gizmos.matrix = transform.localToWorldMatrix;
            Vector2 offset = (Vector2)transform.position * 0;
            foreach (List<Vector2> polygon in polygons)
            {
                for (int i = 0; i < polygon.Count; i++)
                {
                    if (i + 1 == polygon.Count)
                    {
                        Gizmos.DrawLine(polygon[i] + offset, polygon[0] + offset);
                    }
                    else
                    {
                        Gizmos.DrawLine(polygon[i] + offset, polygon[i + 1] + offset);
                    }
                }
            }
        }
    }
}
