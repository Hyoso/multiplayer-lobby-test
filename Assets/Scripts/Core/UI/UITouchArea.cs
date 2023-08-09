using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UITouchArea : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
	private static bool s_pressed;

	public enum PressTypes
	{
		UP,
		DOWN
	}

	public delegate void InputEvent(PressTypes pressType);
	public static event InputEvent Input;
	public static void SendInputEvent(PressTypes pressType)
	{
		Input?.Invoke(pressType);
	}

	private void OnDestroy()
	{
		if (s_pressed)
		{
			PointerUp();
		}
	}

	private void OnDisable()
	{
		if (s_pressed)
		{
			PointerUp();
		}
	}


	public void OnPointerDown(PointerEventData eventData)
	{
		PointerDown();
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		PointerUp();
	}

	private void PointerDown()
	{
		s_pressed = true;
		SendInputEvent(PressTypes.DOWN);
	}

	private void PointerUp()
	{
		s_pressed = false;
		SendInputEvent(PressTypes.UP);
	}
}
