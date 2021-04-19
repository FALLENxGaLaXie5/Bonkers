using UnityEngine;
using System.Collections;

public class LayerIDHandler : MonoBehaviour
{

    void Start()
    {
        this.GetComponent<Renderer>().sortingLayerID =
        this.transform.parent.GetComponent<Renderer>().sortingLayerID;
    }
}