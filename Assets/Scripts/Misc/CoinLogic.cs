using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinLogic : MonoBehaviour
{
    public Animator animatorComponent;

    //this is the value of this coin
    public int coinValue = 5;


    // Start is called before the first frame update
    void Start()
    {
        animatorComponent = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
