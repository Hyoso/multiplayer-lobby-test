using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GizmosUtils
{
    public static void DrawBoundingBox(Bounds bounds, Color color)
    {
        Color baseCol = Gizmos.color;
        Gizmos.color = color;

        Vector3 center = bounds.center;
        Vector3 size = bounds.size;

        Vector3 halfSize = size * 0.5f;

        // Define the eight corners of the bounding box
        Vector3[] corners = new Vector3[8];
        corners[0] = center + new Vector3(-halfSize.x, -halfSize.y, -halfSize.z);
        corners[1] = center + new Vector3(halfSize.x, -halfSize.y, -halfSize.z);
        corners[2] = center + new Vector3(halfSize.x, -halfSize.y, halfSize.z);
        corners[3] = center + new Vector3(-halfSize.x, -halfSize.y, halfSize.z);
        corners[4] = center + new Vector3(-halfSize.x, halfSize.y, -halfSize.z);
        corners[5] = center + new Vector3(halfSize.x, halfSize.y, -halfSize.z);
        corners[6] = center + new Vector3(halfSize.x, halfSize.y, halfSize.z);
        corners[7] = center + new Vector3(-halfSize.x, halfSize.y, halfSize.z);

        // Draw lines connecting the corners to form the bounding box
        Debug.DrawLine(corners[0], corners[1]);
        Debug.DrawLine(corners[1], corners[2]);
        Debug.DrawLine(corners[2], corners[3]);
        Debug.DrawLine(corners[3], corners[0]);

        Debug.DrawLine(corners[4], corners[5]);
        Debug.DrawLine(corners[5], corners[6]);
        Debug.DrawLine(corners[6], corners[7]);
        Debug.DrawLine(corners[7], corners[4]);

        Debug.DrawLine(corners[0], corners[4]);
        Debug.DrawLine(corners[1], corners[5]);
        Debug.DrawLine(corners[2], corners[6]);
        Debug.DrawLine(corners[3], corners[7]);

        Gizmos.color = baseCol;
    }
}
