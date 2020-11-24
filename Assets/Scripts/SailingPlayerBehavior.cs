using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SailingPlayerBehavior : PlayerBehavior
{
    [SerializeField]
    private GameObject m_playerShip;

    private ThirdPersonCamera m_tpsCam;
    private ShipMovement m_playerShipMovement;

    private void Start()
    {
        m_tpsCam = Camera.main.GetComponent<ThirdPersonCamera> ();
        m_playerShipMovement = m_playerShip.GetComponent<ShipMovement> ();
    }

    private void Update ()
    {
        // Camera Control
        float horizontalCameraRotation = Input.GetAxis ("Mouse X");
        float verticalCameraRotation = Input.GetAxis ("Mouse Y");
        m_tpsCam.Rotate (horizontalCameraRotation, verticalCameraRotation, 0.0f);

        float zoomDirection = Input.mouseScrollDelta.y;
        m_tpsCam.Zoom (zoomDirection);

        // Player Ship Control
        int shipMove = 0;

        if (Input.GetKeyDown (KeyCode.W))
        {
            shipMove += 1;
        }

        if (Input.GetKeyDown (KeyCode.S))
        {
            shipMove -= 1;
        }

        shipMove += (int) m_playerShipMovement.MoveState;
        m_playerShipMovement.MoveState = (ShipMoveState) Mathf.Clamp (shipMove, (int) ShipMoveState.Stop, (int) ShipMoveState.Fast);

        float shipRotateDirection = 0.0f;

        if (Input.GetKey (KeyCode.D))
        {
            shipRotateDirection += 1.0f;
        }
        if (Input.GetKey (KeyCode.A))
        {
            shipRotateDirection -= 1.0f;
        }

        m_playerShipMovement.Rotate (shipRotateDirection);
    }
}
