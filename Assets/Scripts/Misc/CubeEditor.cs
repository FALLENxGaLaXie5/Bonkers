using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[SelectionBase]
[RequireComponent(typeof(Waypoint))]
public class CubeEditor : MonoBehaviour
{

    Waypoint waypoint;

    private void Awake()
    {
        waypoint = GetComponent<Waypoint>();
    }

#if UNITY_EDITOR
    void Update()
    {
        
        //Code here for Editor only.
        SnapToGrid();
        //UpdateLabel();
    }
#endif


    private void SnapToGrid()
    {
        int gridSize = waypoint.GetGridSize();
        transform.position = new Vector3(
            waypoint.GetGridPos().x * gridSize,
            waypoint.GetGridPos().y * gridSize, 0f
            
        );
    }

    //will throw errors at the moment
    private void UpdateLabel()
    {
        TextMesh textMesh = GetComponentInChildren<TextMesh>();
        string labelText =
            waypoint.GetGridPos().x +
            "," +
            waypoint.GetGridPos().y;
        textMesh.text = labelText;
        gameObject.name = labelText;
    }
}