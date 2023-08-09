using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ObjectUtilsEditor : Editor
{
	[MenuItem("CustomUtilities/GameObject/Set name to positionXY %'")]
	public static void SetNameToPositionXY()
	{
		Transform t = Selection.activeTransform as Transform;

		if (t == null) return;
		t.gameObject.name = "x:" + t.transform.position.x + " y:" + t.transform.position.y;
	}
}
