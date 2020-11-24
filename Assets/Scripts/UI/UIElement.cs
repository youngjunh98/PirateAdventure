using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIElement : MonoBehaviour
{
    [SerializeField]
    private string m_uiManagerGameObjectName = "UI Manager";

    private UIManager m_uiManager;

    protected UIManager UIManager
    {
        get
        {
            if (!m_uiManager)
            {
                m_uiManager = GameObject.Find (m_uiManagerGameObjectName)?.GetComponent<UIManager> ();
            }

            return m_uiManager;
        }
    }

    protected virtual void Awake ()
    {
        UIManager.RegisterElement (this);
    }

    protected virtual void OnDestroy ()
    {
        UIManager?.UnregisterElement (this);
    }
}
