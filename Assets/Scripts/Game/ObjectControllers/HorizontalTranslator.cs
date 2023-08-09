using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalTranslator : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed of the horizontal movement
    public float distanceToMove = 10f; // Total distance to move before resetting
    
    private Vector3 initialPosition;
    private float distanceMoved = 0f;

    void Start()
    {
        initialPosition = transform.position;
    }

    void Update()
    {
        // Calculate the distance moved in the current frame
        float distanceThisFrame = moveSpeed * Time.deltaTime;

        // Move the object horizontally
        transform.Translate(Vector3.right * distanceThisFrame);

        // Update the total distance moved
        distanceMoved += Mathf.Abs(distanceThisFrame);

        // Check if the total distance moved is greater than or equal to the distanceToMove
        if (distanceMoved >= distanceToMove)
        {
            // Reset the position to the initial position
            transform.position = initialPosition;

            // Reset the distanceMoved variable
            distanceMoved = 0f;
        }
    }
}
