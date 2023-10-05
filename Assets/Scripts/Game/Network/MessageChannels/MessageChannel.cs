using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using static AuthenticationServiceFacade;

public class DisposableSubscription<T> : IDisposable
{
    Action<T> m_Handler;
    bool m_IsDisposed;
    IMessageChannel<T> m_MessageChannel;

    public DisposableSubscription(IMessageChannel<T> messageChannel, Action<T> handler)
    {
        m_MessageChannel = messageChannel;
        m_Handler = handler;
    }

    public void Dispose()
    {
        if (!m_IsDisposed)
        {
            m_IsDisposed = true;

            if (!m_MessageChannel.IsDisposed)
            {
                m_MessageChannel.Unsubscribe(m_Handler);
            }

            m_Handler = null;
            m_MessageChannel = null;
        }
    }
}

public interface ISubscriber<T>
{
    IDisposable Subscribe(Action<T> handler);
    void Unsubscribe(Action<T> handler);
}

public interface IMessageChannel<T> : IPublisher<T>, ISubscriber<T>, IDisposable
{
    bool IsDisposed { get; }
}

public class MessageChannel<T> : IMessageChannel<T>
{
    readonly List<Action<T>> m_MessageHandlers = new List<Action<T>>();

    /// This dictionary of handlers to be either added or removed is used to prevent problems from immediate
    /// modification of the list of subscribers. It could happen if one decides to unsubscribe in a message handler
    /// etc.A true value means this handler should be added, and a false one means it should be removed
    readonly Dictionary<Action<T>, bool> m_PendingHandlers = new Dictionary<Action<T>, bool>();

    public bool IsDisposed { get; private set; } = false;

    public virtual void Dispose()
    {
        if (!IsDisposed)
        {
            IsDisposed = true;
            m_MessageHandlers.Clear();
            m_PendingHandlers.Clear();
        }
    }

    public virtual void Publish(T message)
    {
        foreach (var handler in m_PendingHandlers.Keys)
        {
            if (m_PendingHandlers[handler])
            {
                m_MessageHandlers.Add(handler);
            }
            else
            {
                m_MessageHandlers.Remove(handler);
            }
        }
        m_PendingHandlers.Clear();

        foreach (var messageHandler in m_MessageHandlers)
        {
            if (messageHandler != null)
            {
                messageHandler.Invoke(message);
            }
        }
    }

    public virtual IDisposable Subscribe(Action<T> handler)
    {
        Assert.IsTrue(!IsSubscribed(handler), "Attempting to subscribe with the same handler more than once");

        if (m_PendingHandlers.ContainsKey(handler))
        {
            if (!m_PendingHandlers[handler])
            {
                m_PendingHandlers.Remove(handler);
            }
        }
        else
        {
            m_PendingHandlers[handler] = true;
        }

        var subscription = new DisposableSubscription<T>(this, handler);
        return subscription;
    }

    public void Unsubscribe(Action<T> handler)
    {
        if (IsSubscribed(handler))
        {
            if (m_PendingHandlers.ContainsKey(handler))
            {
                if (m_PendingHandlers[handler])
                {
                    m_PendingHandlers.Remove(handler);
                }
            }
            else
            {
                m_PendingHandlers[handler] = false;
            }
        }
    }

    bool IsSubscribed(Action<T> handler)
    {
        var isPendingRemoval = m_PendingHandlers.ContainsKey(handler) && !m_PendingHandlers[handler];
        var isPendingAdding = m_PendingHandlers.ContainsKey(handler) && m_PendingHandlers[handler];
        return m_MessageHandlers.Contains(handler) && !isPendingRemoval || isPendingAdding;
    }
}