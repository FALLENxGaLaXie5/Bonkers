using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExplodableFragments : ExplodableAddon{
    public override void OnFragmentsGenerated(List<GameObject> fragments)
    {
        foreach (GameObject fragment in fragments)
        {
            Breakable fragExp = fragment.AddComponent<Breakable>();
            fragExp.shatterType = Breakable.shatterType;
            fragExp.fragmentLayer = Breakable.fragmentLayer;
            fragExp.sortingLayerName = Breakable.sortingLayerName;
            fragExp.orderInLayer = Breakable.orderInLayer;

            fragment.layer = Breakable.gameObject.layer;

            fragExp.FragmentInEditor();
        }
    }
}
