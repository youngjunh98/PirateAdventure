using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SailingGameRule : GameRule
{
    [SerializeField]
    private GameObject m_playerShip;
    [SerializeField]
    private float m_initialZoomDistance;

    private void Start ()
    {
        var tpsCam = Camera.main.GetComponent<ThirdPersonCamera> ();
        tpsCam.TargetTransform = m_playerShip.GetComponentInChildren<ThirdPersonCameraTarget> ().transform;
        tpsCam.Distance = m_initialZoomDistance;
    }
}
