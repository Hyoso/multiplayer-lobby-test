using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
	public Canvas canvas { get { return m_canvas; } set { } }

	[SerializeField] private Canvas m_canvas;
    [SerializeField] private int m_levelOverride = -1;

    private LevelRoot m_levelRoot;
    private int m_level;

    protected override void Init()
	{
		ProjectScreenManager.PushScreen(ProjectScreenManager.ScreenIDs.NETWORK_SCREEN);
	}

    private void Awake()
    {
		InitSaveData();
		ProjectScreenManager.PushScreen(ProjectScreenManager.ScreenIDs.NETWORK_SCREEN);
    }

    private void Start()
    {
		GDPRManager.Instance.CheckShowGDPR();
    }

    public void StartGame()
	{
        LoadLevel();

        ProjectScreenManager.ReplaceScreen(ProjectScreenManager.ScreenIDs.GAME_SCREEN);
		GameplayEvents.SendStartGameEvent();
	}

	public void LevelComplete()
	{
        IncrementLevelCounter();


        ProjectScreenManager.ReplaceScreen(ProjectScreenManager.ScreenIDs.LEVEL_COMPLETE_SCREEN);
		GameplayEvents.SendLevelCompleteEvent();
	}

	public void GameOver()
	{
		ProjectScreenManager.ReplaceScreen(ProjectScreenManager.ScreenIDs.GAME_OVER_SCREEN);
		GameplayEvents.SendGameOverEvent();
	}

	public void RestartGame()
	{
		ProjectScreenManager.ReplaceScreen(ProjectScreenManager.ScreenIDs.START_SCREEN);
		GameplayEvents.SendRestartGameEvent();

		ProjectSceneManager.Instance.UnloadLastScene();
        m_levelRoot = null;
    }

    public void RegisterLevelRoot(LevelRoot root)
    {
        m_levelRoot = root;
    }

    private void InitSaveData()
    {
        m_level = SaveSystem.Instance.GetInt(BucketGameplay.LEVEL);
    }

    private void LoadLevel()
    {
        int lvl = m_level % AssetsConfig.Instance.levels.Count;

        if (m_levelOverride != -1)
        {
            lvl = m_levelOverride;
        }

        ProjectSceneManager.Instance.LoadScene(AssetsConfig.Instance.levelNames[lvl]);
    }

    private void IncrementLevelCounter()
    {
        m_level++;

        SaveSystem.Instance.SetInt(BucketGameplay.LEVEL, "", m_level);
        SaveSystem.Instance.Save();
    }
}
