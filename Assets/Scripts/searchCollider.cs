using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class searchCollider : MonoBehaviour
{
    private searchScript searchScriptParent;

    private void OnTriggerEnter(Collider other)
    {
        searchScriptParent.CollisionEnter(other);
    }


    private void OnTriggerExit(Collider other)
    {
        searchScriptParent.CollisionExit(other);
    }


    private void Awake()
    {
        searchScriptParent = transform.parent.GetComponent<searchScript>(); // assigning a Parental search script to a variable
    }
}
