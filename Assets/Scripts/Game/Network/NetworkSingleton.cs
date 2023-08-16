using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class NetworkSingleton<T> : NetworkBehaviour
    where T : NetworkSingleton<T>
{
    protected static T s_instance = null;
    private static bool m_initialised = false;

    protected abstract void Init();

    public static T Instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = GameObject.FindObjectOfType(typeof(T)) as T;

                if (s_instance == null)
                {
                    s_instance = new GameObject(typeof(T).ToString() + "_Spawned", typeof(T)).GetComponent<T>();
                }
            }

            if (!m_initialised)
            {
                m_initialised = true;
                s_instance.Init();
                DontDestroyOnLoad(s_instance.gameObject);
            }

            return s_instance;
        }

        private set { }
    }

    private void Awake()
    {
        if (s_instance == null)
        {
            s_instance = this as T;
        }
        else if (s_instance != this)
        {
            Debug.LogError("Another instance of " + GetType() + " already exists");
            Destroy(this.gameObject);
            return;
        }

        if (!m_initialised)
        {
            Init();
            m_initialised = true;
            DontDestroyOnLoad(this.gameObject);
        }
    }
}
