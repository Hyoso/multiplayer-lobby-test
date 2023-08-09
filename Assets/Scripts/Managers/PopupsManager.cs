using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class PopupsManager : Singleton<PopupsManager>
{
    [SerializeField] private Transform m_popupsParent;
    [SerializeField] private List<GameObject> m_popups = new List<GameObject>();

    protected override void Init()
    {
    }

    public void CloseAllPopups()
    {
        foreach (var item in m_popups)
        {
            Popup popup = item.GetComponent<Popup>();
            popup.CloseWindow();
        }
    }

    public GameObject CreatePopup(string path)
    {
        Transform canvasTransform = GameManager.Instance.canvas.transform;
        GameObject popup = Instantiate(Resources.Load<GameObject>(path), canvasTransform);
        m_popups.Add(popup);

        popup.transform.SetAsLastSibling();

        return popup;
    }

    public void RemovePopup(GameObject obj, float delay = 0f)
    {
        if (m_popups.Contains(obj))
        {
            if (delay == 0f)
            {
                Destroy(obj);
                m_popups.Remove(obj);
            }
            else
            {
                StartCoroutine(DelayedRemovePopupRoutine(obj, delay));
            }
        }
    }

    private IEnumerator DelayedRemovePopupRoutine(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(obj);
        m_popups.Remove(obj);
    }
}