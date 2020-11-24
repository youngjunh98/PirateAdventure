using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleGameRule : GameRule
{
    [SerializeField]
    private string m_playSceneName;
    [SerializeField]
    private Vector3 m_tpsCamAngles;

    private void Start ()
    {
        var tpsCam = Camera.main.GetComponent<ThirdPersonCamera> ();
        tpsCam.PolarAngle = m_tpsCamAngles.x;
        tpsCam.AzimuthalAngle = m_tpsCamAngles.y;
        tpsCam.Roll = m_tpsCamAngles.z;
    }

    private void OnEnable ()
    {
        GameController.UIManager.FindElement ("Play Button", true).GetComponent<Button> ().onClick.AddListener (OnClickPlayButton);
    }

    private void OnDisable ()
    {
        GameController?.UIManager?.FindElement ("Play Button", true).GetComponent<Button> ().onClick.RemoveListener (OnClickPlayButton);
    }

    public void OnClickPlayButton ()
    {
        GameController.UIManager.FindElement (UICanvasName, false).gameObject.SetActive (false);
        GameController.SceneController.RequestUnloadScene (gameObject.scene.name);
        GameController.SceneController.RequestLoadScene (m_playSceneName, true);
    }
}
