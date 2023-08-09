using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class GameplayEvents
{
	public delegate void BasicDelegate();
	public delegate void IntDelegate(int intValue);
	public delegate void StringDelegate(string stringValue);
	public delegate void BoolDelegate(bool boolValue);
	public delegate void FloatDelegate(float floatValue);
	public delegate void LongDelegate(long longValue);

	public static event BasicDelegate StartGameEvent;
	public static void SendStartGameEvent()
	{
		StartGameEvent?.Invoke();
	}

	public static event BasicDelegate LevelCompleteEvent;
	public static void SendLevelCompleteEvent()
	{
		LevelCompleteEvent?.Invoke();
	}

	public static event BasicDelegate GameOverEvent;
	public static void SendGameOverEvent()
	{
		GameOverEvent?.Invoke();
	}

	public static event BasicDelegate RestartGameEvent;
	public static void SendRestartGameEvent()
	{
		RestartGameEvent?.Invoke();
	}

	public static event IntDelegate GenericAdEvent;
	public static void SendGenericAdEvent(int eventId)
	{
		GenericAdEvent?.Invoke(eventId);
	}

	public static event BasicDelegate LevelLoadedEvent;
	public static void SendLevelLoadedEvent()
	{
		LevelLoadedEvent?.Invoke();
	}
}
