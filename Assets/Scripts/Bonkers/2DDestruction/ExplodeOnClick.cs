using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Breakable))]
public class ExplodeOnClick : MonoBehaviour {

	private Breakable breakable;

	void Start()
	{
		breakable = GetComponent<Breakable>();
	}
	void OnMouseDown()
	{
		breakable.ActivateFragments();
		BreakForce ef = GameObject.FindObjectOfType<BreakForce>();
		ef.DoBreaking(transform.position);
	}
}
