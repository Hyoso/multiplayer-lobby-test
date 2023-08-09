using UnityEngine;

public static class TransformUtils
{
	public static void DestroyAllChildren(this Transform transform)
	{
		foreach (Transform child in transform)
		{
			GameObject.Destroy(child.gameObject);
		}
	}
}
