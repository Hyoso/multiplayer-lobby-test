using System.ComponentModel;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
	public Canvas canvas { get { return m_canvas; } set { } }
    public PlayerController localPlayer { get { return m_networkPlayerController; } private set { } }


	[SerializeField] private Canvas m_canvas;
    [SerializeField] private int m_levelOverride = -1;
    [SerializeField] private PlayerCamera m_playerCam;
    [SerializeField, NaughtyAttributes.ReadOnly] private PlayerController m_networkPlayerController;

    private LevelRoot m_levelRoot;
    private int m_level;

    protected override void Init()
	{
		ProjectScreenManager.PushScreen(ProjectScreenManager.ScreenIDs.NETWORK_SCREEN);
	}

    private void Awake()
    {
		InitSaveData();
    }

    private void Start()
    {
		GDPRManager.Instance.CheckShowGDPR();

        GameNetworkManager.Instance.StartOffline();
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

    public void SetupPlayerCam(GameObject player)
    {
        m_playerCam.AssignPlayer(player);
    }

    public void RegisterLocalPlayer(PlayerController controller)
    {
        m_networkPlayerController = controller;
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
