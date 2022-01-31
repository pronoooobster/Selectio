using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class colorChangeTest : MonoBehaviour
{
    
    void FixedUpdate()
    {
        this.GetComponent<MeshRenderer>().material.color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1.0f);    
    }
}
