using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

public class ApplicationController : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        base.Configure(builder);
        //these message channels are essential and persist for the lifetime of the lobby and relay services
        // Registering as instance to prevent code stripping on iOS
        builder.RegisterInstance(new MessageChannel<UnityServiceErrorMessage>()).AsImplementedInterfaces();

        //all the lobby service stuff, bound here so that it persists through scene loads
        builder.Register<AuthenticationServiceFacade>(Lifetime.Singleton); //a manager entity that allows us to do anonymous authentication with unity services
    }

    private void Start()
    {
        Application.wantsToQuit += OnWantToQuit;
        DontDestroyOnLoad(gameObject);
        Application.targetFrameRate = 120;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    /// <summary>
    ///     In builds, if we are in a lobby and try to send a Leave request on application quit, it won't go through if we're quitting on the same frame.
    ///     So, we need to delay just briefly to let the request happen (though we don't need to wait for the result).
    /// </summary>
    private IEnumerator LeaveBeforeQuit()
    {
        yield return null;
        Application.Quit();
    }

    private bool OnWantToQuit()
    {
        Application.wantsToQuit -= OnWantToQuit;

        //var canQuit = m_LocalLobby != null && string.IsNullOrEmpty(m_LocalLobby.LobbyID);
        //if (!canQuit)
        //{
        //    StartCoroutine(LeaveBeforeQuit());
        //}

        return true;
    }
}
