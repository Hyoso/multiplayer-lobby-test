using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProjectSceneManager : Singleton<ProjectSceneManager>
{
    private string m_lastSceneLoaded;

    protected override void Init()
    {
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        m_lastSceneLoaded = sceneName;
    }

    public void UnloadLastScene()
	{
        if (!string.IsNullOrEmpty(m_lastSceneLoaded))
            SceneManager.UnloadSceneAsync(m_lastSceneLoaded);
    }

    public void UnloadScene(string sceneName)
    {
        SceneManager.UnloadSceneAsync(sceneName);
    }

}
