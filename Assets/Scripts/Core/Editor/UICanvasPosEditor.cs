using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class UICanvasPosEditor : EditorWindow
{
    [MenuItem ("CustomUtilities/Rect Transform Editor")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(UICanvasPosEditor));
    }

    void OnGUI()
    {
        bool validObject = Selection.activeGameObject;
        RectTransform rect = null;
        if (validObject)
        {
            rect = Selection.activeGameObject.GetComponent<RectTransform>();
        }

        if (validObject && rect)
        {

            GUILayout.Label("Rect Transform Settings", EditorStyles.boldLabel);
            //myString = EditorGUILayout.TextField("Text Field", myString);

            EditorGUILayout.BeginHorizontal();
            
            EditorGUILayout.LabelField("position", GUILayout.MaxWidth(50.0f));

            float x = rect.position.x;
            x = EditorGUILayout.FloatField(x, new GUILayoutOption[]{ GUILayout.ExpandWidth(true) });
            float y = rect.position.y;
            y = EditorGUILayout.FloatField(y, new GUILayoutOption[]{ GUILayout.ExpandWidth(true) });
            float z = rect.position.z;
            z = EditorGUILayout.FloatField(z, new GUILayoutOption[] { GUILayout.ExpandWidth(true) });
            EditorGUILayout.EndHorizontal();

            rect.position = new Vector3(x, y, z);

        }
        else
        {
            GUILayout.Label("Select a UI Object", EditorStyles.boldLabel);
        }


    }
}
