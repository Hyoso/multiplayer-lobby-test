using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ProjectScreenManager
{
	public enum ScreenIDs
	{
		START_SCREEN,
		GAME_SCREEN,
		LEVEL_COMPLETE_SCREEN,
		GAME_OVER_SCREEN
	}

	public static string[] ScreenDirectories = new string[]
	{
		"Prefabs/Screens/StartScreen",
		"Prefabs/Screens/GameScreen",
		"Prefabs/Screens/LevelCompleteScreen",
		"Prefabs/Screens/GameOverScreen"
	};

	public static UIScreen PushScreen(ScreenIDs screen)
	{
		string screenDir = GetScreenDirectory(screen);
		return ScreenManager.Instance.PushScreen(screenDir);
	}

	public static UIScreen ReplaceScreen(ScreenIDs screen)
	{
		string screenDirectory = GetScreenDirectory(screen);
		return ScreenManager.Instance.ReplaceScreen(screenDirectory);
	}

	public static void PopScreen()
	{
		ScreenManager.Instance.PopScreen();
	}

	public static string GetScreenDirectory(ScreenIDs screen)
	{
		int screenIdx = (int)screen;
		return ScreenDirectories[screenIdx];
	}
}
