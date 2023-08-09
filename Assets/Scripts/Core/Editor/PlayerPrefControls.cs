using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PlayerPrefControls : Editor
{
    [MenuItem("CustomUtilities/PlayerPrefs/Clear")]
    public static void ClearPlayerStats()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }

    [MenuItem("CustomUtilities/PlayerPrefs/Add Credits")]
    public static void AddCredits()
    {
        PlayerPrefs.SetInt("Credits", 10000);
    }

    [MenuItem("CustomUtilities/PlayerPrefs/Add Bank")]
    public static void AddBank()
    {
        PlayerPrefs.SetInt("Bank", 10000);
    }

    [MenuItem("CustomUtilities/PlayerPrefs/Zero Bank and Credits")]
    public static void ZeroBankAndCredits()
    {
        PlayerPrefs.SetInt("Bank", 0);
        PlayerPrefs.SetInt("Credits", 0);
    }
}
