using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileOccupation : MonoBehaviour
{
    public bool isOccupied = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetOccupied(bool newBool)
    {
        this.isOccupied = newBool;
    }

    public bool IsOccupied()
    {
        return this.isOccupied;
    }
}
