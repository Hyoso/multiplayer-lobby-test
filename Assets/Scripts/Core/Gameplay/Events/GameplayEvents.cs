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
	public delegate void ActiveTrinketDelegate(ActiveTrinketSO trinket);
	public delegate void PassiveTrinketDelegate(PassiveTrinketSO trinket);

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

    public static event ActiveTrinketDelegate onActiveTrinketEquipped;
    public static void SendOnActiveTrinketEquippedEvent(ActiveTrinketSO trinket)
    {
        onActiveTrinketEquipped?.Invoke(trinket);
    }

    public static event PassiveTrinketDelegate onPassiveTrinketEquipped;
    public static void SendOnPassiveTrinketEquippedEvent(PassiveTrinketSO trinket)
    {
        onPassiveTrinketEquipped?.Invoke(trinket);
    }

	public static event BasicDelegate UseActiveSkillEvent;
	public static void SendUseActiveSkillEvent()
	{
		UseActiveSkillEvent?.Invoke();
	}

	public static event BasicDelegate InteractWithNPCEvent;
	public static void SendInteractWithNPCEvent()
	{
		InteractWithNPCEvent?.Invoke();
    }


    public static event BasicDelegate GenerateNewLevelEvent;
	public static void SendGenerateNewLevelEvent()
	{
		GenerateNewLevelEvent?.Invoke();
    }

	public static event BoolDelegate PlayerInNPCRangeEvent;
	public static void SendPlayerInNPCRangeEvent(bool inRange)
	{
		PlayerInNPCRangeEvent?.Invoke(inRange);
	}
}
