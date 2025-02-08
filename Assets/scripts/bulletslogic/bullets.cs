using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;

public class bullets : pointandshoot
{
    
    void Start()
    {


        
    }
    void Update()
    {
     if (Input.GetMouseButtonDown(0))
        {   
                  ammoUsed(1);
        }
    }
}