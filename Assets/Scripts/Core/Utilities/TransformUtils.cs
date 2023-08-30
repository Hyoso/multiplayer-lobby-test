using UnityEngine;

public static class TransformUtils
{
    public static void DestroyAllChildren(this Transform transform)
    {
        int childCount = transform.childCount;

        for (int i = childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            GameObject.Destroy(child.gameObject);
        }
    }
    public static void DestroyImmediateAllChildren(this Transform transform)
    {
        int childCount = transform.childCount;

        for (int i = childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            GameObject.DestroyImmediate(child.gameObject);
        }
    }
}
