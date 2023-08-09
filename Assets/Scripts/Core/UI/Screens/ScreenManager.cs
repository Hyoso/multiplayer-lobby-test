using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenManager : Singleton<ScreenManager>
{
	private Canvas m_canvas;
	private CanvasScaler m_canvasScaler;

	private Stack<UIScreen> m_screenStack = new Stack<UIScreen>();
	private List<UIScreen> m_screensToDestroy = new List<UIScreen>();

	protected override void Init()
	{
	}

	public void SetCanvas(Canvas canvas)
	{
		m_canvas = canvas;
		m_canvasScaler = m_canvas.GetComponent<CanvasScaler>();
	}

	public UIScreen GetCurrentScreen()
	{
		UIScreen screen = null;
		if (m_screenStack.Count > 0)
		{
			screen = m_screenStack.Peek();
		}

		return screen;
	}

	public UIScreen GetScreenPrefab(string prefabName)
	{
		GameObject prefab = (GameObject)Resources.Load(prefabName);
		return prefab.GetComponent<UIScreen>();
	}

	public UIScreen PushScreen(string screenDir)
	{
		UIScreen prefab = GetScreenPrefab(screenDir);
		return PushScreen(prefab);
	}

	public UIScreen PushScreen(UIScreen screen)
	{
		UIScreen newScreen = (UIScreen)Instantiate(screen, m_canvas.transform, false);
		UIScreen oldScreen = null;
		if (m_screenStack.Count > 0)
		{
			oldScreen = m_screenStack.Peek();
		}

		if (newScreen)
		{
			m_screenStack.Push(newScreen);
		}

		SwapScreens(oldScreen, newScreen);

		return newScreen;
	}

	public UIScreen PopScreen()
	{
		UIScreen prevScreen = m_screenStack.Pop();
		UIScreen newScreen = null;

		if (m_screenStack.Count > 0)
		{
			newScreen = m_screenStack.Peek();
		}

		m_screensToDestroy.Add(prevScreen);

		SwapScreens(prevScreen, newScreen);

		return newScreen;
	}

	public UIScreen ReplaceAllScreens(string screenDir)
	{
		UIScreen prefab = GetScreenPrefab(screenDir);
		return ReplaceAllScreens(prefab);
	}

	public UIScreen ReplaceScreen(string screenDir)
	{
		UIScreen prefab = GetScreenPrefab(screenDir);
		return ReplaceScreen(prefab);
	}

	public UIScreen ReplaceAllScreens(UIScreen screen)
	{
		UIScreen newScreen = (UIScreen)Instantiate(screen, m_canvas.transform, false);
		DestroyScreens();
		m_screenStack.Push(newScreen);
		return newScreen;
	}

	public UIScreen ReplaceScreen(UIScreen screen)
	{
		UIScreen newScreen = (UIScreen)Instantiate(screen, m_canvas.transform, false);
		UIScreen oldScreen = m_screenStack.Pop();

		m_screensToDestroy.Add(oldScreen);
		m_screenStack.Push(newScreen);

		SwapScreens(oldScreen, newScreen);
		return newScreen;
	}

	public void SwapScreens(UIScreen oldScreen, UIScreen newScreen)
	{
		oldScreen?.gameObject.SetActive(false);
		newScreen?.gameObject.SetActive(true);

		DestroyScreens();
	}

	private void DestroyScreens()
	{
		foreach (UIScreen screen in m_screensToDestroy)
		{
			if (screen)
			{
				Destroy(screen.gameObject);
			}
		}
		m_screensToDestroy.Clear();
	}
}
