using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputControlTester : MonoBehaviour, IOffsetController
{
	private OffsetInput m_offsetInput;

	private void Awake()
	{
		m_offsetInput = GetComponent<OffsetInput>();
		m_offsetInput.RegisterController(this);
	}

	private void OnDestroy()
	{

	}

	public void JoyControlChanged(Vector2 offset)
	{
		transform.position += new Vector3(offset.x, 0.0f, offset.y);
	}

	public void JoyControlStart()
	{
	}

	public void JoyControlStopped()
	{
	}

	public void OffsetControlChanged(Vector2 offset)
	{
		transform.position = new Vector3(offset.x, 0.0f, offset.y);
	}

	public void OffsetControlStopped()
	{
	}

	public void OffsetControlStart()
	{
	}
}
