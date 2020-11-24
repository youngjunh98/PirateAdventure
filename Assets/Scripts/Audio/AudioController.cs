using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    [SerializeField]
    private AudioSource m_backgroundMusicAudio;
    [SerializeField]
    private AudioSource m_uiAudio;

    public AudioSource BackgroundMusicAudio
    {
        get { return m_backgroundMusicAudio; }
    }

    public AudioSource UIAudio
    {
        get { return m_uiAudio; }
    }
}
