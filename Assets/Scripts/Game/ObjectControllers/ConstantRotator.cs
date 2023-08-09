using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantRotator : MonoBehaviour
{
    public float rotationSpeed = 30f; // Speed of the rotation in degrees per second

    void Update()
    {
        // Rotate the object around the Z-axis (forward direction) at the specified speed
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
    }
}
