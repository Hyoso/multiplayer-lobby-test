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

	public static event StringDelegate onOnlineHostStarted;
	public static void SendonOnlineHostStarted(string joinCode)
	{
		onOnlineHostStarted?.Invoke(joinCode);
    }

	public static event BasicDelegate onOnlineHostStopped;
	public static void SendonOnlineHostStopped()
	{
		onOnlineHostStopped?.Invoke();
    }

	public static event BasicDelegate onHostDisconnected;
	public static void SendonHostDisconnected()
	{
		onHostDisconnected?.Invoke();
    }

    public static event StringDelegate onJoinHostSuccess;
	public static void SendonJoinHostSuccess(string joinCode)
	{
		onJoinHostSuccess?.Invoke(joinCode);
    }

	public static event BasicDelegate onJoinHostAttempt;
    public static void SendonJoinHostAttempt()
    {
        onJoinHostAttempt?.Invoke();
    }
}
