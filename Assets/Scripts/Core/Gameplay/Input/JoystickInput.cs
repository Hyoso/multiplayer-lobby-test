using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IJoyController
{
	void JoyControlChanged(Vector2 offset);
	void JoyControlStopped();
	void JoyControlStart();
}

public class JoystickInput : MonoBehaviour
{
	[SerializeField] private float m_followDistance = 0.2f;
	[SerializeField] private float m_deadZone = 0.05f;
	[SerializeField] private float m_factor = 1f;
	[SerializeField] private Canvas m_canvas;

	private IJoyController m_controller;
	private GameObject m_uiVisualiser;

	private bool m_mouseDown;
	private bool m_moving;
	private Vector3 m_initialMousedownPos;

	public void RegisterController(IJoyController controller)
	{
		m_controller = controller;
	}

	public void DeRegisterController()
	{
		m_controller = null;
	}

	public void RegisterUIVisualiser(GameObject visual)
	{
		m_uiVisualiser = visual;
	}

	private void Start()
	{
		m_controller = GetComponent<IJoyController>();
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
			m_controller.JoyControlStopped();
		}

		m_mouseDown = false;
		m_moving = false;
	}

	private void OnDestroy()
	{
		if (m_mouseDown)
		{
			m_controller.JoyControlStopped();
		}
	}

	private void UITouchArea_Input(UITouchArea.PressTypes pressType)
	{
		if (m_controller != null)
		{
			if (pressType == UITouchArea.PressTypes.DOWN)
			{
				m_mouseDown = true;
				m_controller.JoyControlStart();
				m_initialMousedownPos = Input.mousePosition;
				SetUIVisualiser();
			}
			else
			{
				m_mouseDown = false;
				m_moving = false;
				m_controller.JoyControlStopped();
				HideUIVisualiser();
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
				if (mag > m_followDistance)
				{
					Vector3 normalisedOffset = offset.normalized;
					offset = normalisedOffset * m_followDistance;
					m_initialMousedownPos = Input.mousePosition - (normalisedOffset * m_followDistance * Screen.width);
					SetUIVisualiser();
				}

				Vector3 factorisedOffset = offset * m_factor;
				m_controller.JoyControlChanged(new Vector2(factorisedOffset.x, factorisedOffset.y));
			}
		}
	}

	private void SetUIVisualiser()
	{
		if (m_uiVisualiser != null)
		{
			if (!m_uiVisualiser.activeInHierarchy)
			{
				m_uiVisualiser.SetActive(true);
			}

			Vector3 canvasPosition = CanvasPositioningExtensions.ScreenToCanvasPosition(m_canvas, m_initialMousedownPos);
			m_uiVisualiser.transform.localPosition = canvasPosition;
		}
	}

	private void HideUIVisualiser()
	{
		if (m_uiVisualiser != null)
		{
			m_uiVisualiser.SetActive(false);
		}
	}
}
