using UnityEngine;
using UnityEngine.UI;

public class Popup : MonoBehaviour
{
    public CanvasGroup window;

    private void Awake()
    {
        window = gameObject.GetComponent<CanvasGroup>();
        if (window == null)
        {
            window = gameObject.AddComponent<CanvasGroup>();
        }
    }

    protected virtual void Start()
    {
        ShowWindow();
    }

    public void ShowWindow()
    {
        window.alpha = 1;
        window.interactable = true;
        window.blocksRaycasts = true;
    }

    public void CloseWindow()
    {
        window.alpha = 0;
        window.interactable = false;
        window.blocksRaycasts = false;

        PopupsManager.Instance.RemovePopup(this.gameObject);
    }
}