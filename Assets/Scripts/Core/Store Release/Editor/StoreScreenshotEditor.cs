using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class StoreScreenshotEditor : Editor
{
    [MenuItem("Store/Screenshot")]
    public static void TakeScreenshotAndSave()
    {
        GameObject go = new GameObject("Screenshotter");
        StoreScreenshot ss = go.AddComponent<StoreScreenshot>();
        ss.TakeScreenshot();
    }
}
