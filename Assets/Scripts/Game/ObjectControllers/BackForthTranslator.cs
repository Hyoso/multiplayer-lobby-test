using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackForthTranslator : MonoBehaviour
{
    public enum MovementDirection
    {
        Horizontal,
        Vertical,
        Both
    }

    public float moveSpeed = 5f; // Speed of the movement
    public MovementDirection movementDirection = MovementDirection.Horizontal; // Movement direction enum
    public float minPosition = 0f; // Minimum position along the movement direction
    public float maxPosition = 10f; // Maximum position along the movement direction
    public float holdTime = 1f; // Time to hold at the end points

    private float currentPosition;
    private float direction = 1f;
    private bool isHolding = false;
    private float holdTimer = 0f;

    void Start()
    {
        currentPosition = GetInitialPosition();
    }

    void Update()
    {
        if (!isHolding)
        {
            // Calculate the movement direction based on the current settings
            Vector3 movementDirectionVector = GetMovementDirectionVector();

            // Calculate the distance to move in the current frame
            float distanceThisFrame = moveSpeed * Time.deltaTime * direction;

            // Move the object based on the selected movement direction
            transform.Translate(movementDirectionVector * distanceThisFrame);

            // Update the current position
            currentPosition += distanceThisFrame;

            // Check if the object reaches the minimum or maximum position
            if (currentPosition <= minPosition || currentPosition >= maxPosition)
            {
                // Reverse the direction to move back towards the opposite end
                direction = -direction;

                // Start holding at the end point
                isHolding = true;
                holdTimer = 0f;
            }
        }

        // If holding, increment the hold timer
        if (isHolding)
        {
            holdTimer += Time.deltaTime;

            // Check if the hold time is over
            if (holdTimer >= holdTime)
            {
                isHolding = false;
            }
        }
    }

    // Helper function to get the initial position along the movement direction
    private float GetInitialPosition()
    {
        switch (movementDirection)
        {
            case MovementDirection.Horizontal:
                return transform.position.x;
            case MovementDirection.Vertical:
                return transform.position.y;
            case MovementDirection.Both:
                return movementDirection == MovementDirection.Horizontal ? transform.position.x : transform.position.y;
            default:
                return 0f;
        }
    }

    // Helper function to get the movement direction vector based on the selected movement direction
    private Vector3 GetMovementDirectionVector()
    {
        switch (movementDirection)
        {
            case MovementDirection.Horizontal:
                return Vector3.right;
            case MovementDirection.Vertical:
                return Vector3.up;
            case MovementDirection.Both:
                return new Vector3(1f, 1f, 0f).normalized;
            default:
                return Vector3.zero;
        }
    }
}
