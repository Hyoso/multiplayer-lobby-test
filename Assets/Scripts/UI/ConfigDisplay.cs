using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConfigDisplay : MonoBehaviour
{
	private void Awake()
	{
		TextMeshPro t = GetComponent<TextMeshPro>();
		t.text = GameplayConfig.Instance.testFloat.ToString();
	}
}
