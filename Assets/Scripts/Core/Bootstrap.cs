using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bootstrap : MonoBehaviour
{
	[SerializeField] private Canvas m_canvas;
	[SerializeField] private List<GameObject> m_executionOrder;

	private void Awake()
	{
		ScreenManager.Instance.SetCanvas(m_canvas);

		DoExecutionOrder();
	}

	private void DoExecutionOrder()
	{
		foreach (var item in m_executionOrder)
		{
			item.SetActive(true);
		}		
	}
}
