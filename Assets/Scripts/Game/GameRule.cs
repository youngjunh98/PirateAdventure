using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameRule : MonoBehaviour
{
    [SerializeField]
    private string m_uiCanvasName;
    [SerializeField]
    private AudioClip m_backgroundMusicClip;

    public GameController GameController { get; private set; }
    public string UICanvasName { get => m_uiCanvasName; }

    protected virtual void Awake ()
    {
        GameController = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController> ();
        GameController.UIManager.FindElement (m_uiCanvasName, true)?.gameObject.SetActive (true);

        if (m_backgroundMusicClip)
        {
            GameController.AudioController.BackgroundMusicAudio.clip = m_backgroundMusicClip;
            GameController.AudioController.BackgroundMusicAudio.Play ();
        }
    }
}
