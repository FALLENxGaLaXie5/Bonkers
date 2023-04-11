using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent(typeof(Breakable))]
public abstract class ExplodableAddon : MonoBehaviour {
    protected Breakable Breakable;
	// Use this for initialization
	void Start () {
        Breakable = GetComponent<Breakable>();
	}

    public abstract void OnFragmentsGenerated(List<GameObject> fragments);
}
