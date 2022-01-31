using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraRotation : MonoBehaviour
{
    public float rotationSpeed = 10.0f;

    private void Update()
    {
        transform.Rotate(Vector3.up, Input.GetAxis("Horizontal") * rotationSpeed * Time.unscaledDeltaTime);
    }
}
