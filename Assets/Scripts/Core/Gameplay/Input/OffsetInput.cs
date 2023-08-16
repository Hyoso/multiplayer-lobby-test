using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IOffsetController
{
	void OffsetControlChanged(Vector2 offset);
	void OffsetControlStopped();
	void OffsetControlStart();
}

public class OffsetInput : MonoBehaviour
{
	[SerializeField] private float m_deadZone = 0.05f;
	[SerializeField] private float m_factor = 1f;

	private IOffsetController m_controller;

	private bool m_mouseDown;
	private bool m_moving;
	private Vector3 m_initialMousedownPos;

	public void RegisterController(IOffsetController controller)
	{
		m_controller = controller;
	}

	public void DeRegisterController()
	{
		m_controller = null;
	}

	private void OnEnable()
	{
		UITouchArea.Input += UITouchArea_Input;
	}

	private void OnDisable()
	{
		UITouchArea.Input -= UITouchArea_Input;

		if (m_mouseDown)
		{
			m_controller.OffsetControlStopped();
		}

		m_mouseDown = false;
		m_moving = false;
	}

	private void OnDestroy()
	{
		if (m_mouseDown)
		{
			m_controller.OffsetControlStopped();
		}
	}

	private void UITouchArea_Input(UITouchArea.PressTypes pressType)
	{
		if (m_controller != null)
		{
			if (pressType == UITouchArea.PressTypes.DOWN)
			{
				m_mouseDown = true;
				m_controller.OffsetControlStart();
				m_initialMousedownPos = Input.mousePosition;
			}
			else
			{
				m_mouseDown = false;
				m_moving = false;
				m_controller.OffsetControlStopped();
			}
		}
	}

	private void Update()
	{
		UpdateJoystick();
	}

	private void UpdateJoystick()
	{
		if (m_mouseDown && m_controller != null)
		{
			Vector3 offset = Input.mousePosition - m_initialMousedownPos;
			offset /= Screen.width;

			float mag = offset.magnitude;
			if (mag > m_deadZone)
			{
				m_moving = true;
			}

			if (m_moving)
			{
				Vector3 factorisedOffset = offset * m_factor;
				m_controller.OffsetControlChanged(new Vector2(factorisedOffset.x, factorisedOffset.y));
			}
		}
	}
}
