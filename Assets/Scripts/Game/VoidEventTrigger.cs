using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidEventTrigger : MonoBehaviour
{
	[SerializeField] private VoidEvent m_event;
	[SerializeField] private bool m_sendEvent;

	private void Update()
	{
		if (m_sendEvent)
		{
			m_sendEvent = false;
			m_event.SendEvent();
		}
	}

}
