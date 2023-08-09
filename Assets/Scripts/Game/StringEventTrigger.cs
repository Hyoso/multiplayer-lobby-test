using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringEventTrigger : MonoBehaviour
{
	[SerializeField] private StringEvent m_stringEvent;
	[SerializeField] private string m_sendText;
	[SerializeField] private bool m_send;

	private void Update()
	{
		if (m_send)
		{
			m_send = false;

			m_stringEvent.SendEvent(m_sendText);
		}
	}
}
