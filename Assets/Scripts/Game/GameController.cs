using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private string m_titleSceneName;

    [Header("UI")]
    [SerializeField]
    private string m_uiSceneName;
    [SerializeField]
    private string m_uiManagerName;

    public SceneController SceneController { get; private set; }
    public AudioController AudioController { get; private set; }
    public UIManager UIManager { get; private set; }

    private void Awake ()
    {
        SceneController = GetComponent<SceneController> ();
        AudioController = GetComponent<AudioController> ();

        SceneController.OnLoadEndEventDictionary.GetEvent (m_uiSceneName).AddListener (OnUISceneLoadEnd);
        SceneController.RequestLoadScene (m_uiSceneName, true);

        SceneController.RequestLoadScene (m_titleSceneName, true);
    }

    private void OnUISceneLoadEnd ()
    {
        UIManager = GameObject.Find (m_uiManagerName).GetComponent<UIManager> ();
    }
}
