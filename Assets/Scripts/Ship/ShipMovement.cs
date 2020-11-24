using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShipMoveState { Stop, Slow, Fast }

public class ShipMovement : MonoBehaviour
{
    [SerializeField]
    private float slowMovePower;
    [SerializeField]
    private float fastMovePower;
    [SerializeField]
    private float rotatePower;

    private ShipMoveState moveState;
    private float rotateDirection;

    private Rigidbody shipRigidbody;
    private ShipMast mast;
    private ShipWheel wheel;

    public ShipMoveState MoveState
    {
        get => moveState;

        set
        {
            moveState = value;
            if (moveState == ShipMoveState.Stop)
            {
                mast.IsUp = true;
            }
            else
            {
                mast.IsUp = false;
            }
        }
    }
    private float ShipMass
    {
        get { return shipRigidbody.mass; }
    }

    void Awake ()
    {
        shipRigidbody = GetComponent<Rigidbody> ();
        mast = GetComponent<ShipMast> ();
        wheel = GetComponent<ShipWheel> ();
    }

    void Start ()
    {
        MoveState = ShipMoveState.Stop;
        rotateDirection = 0.0f;
    }

    void Update ()
    {
        if (wheel != null)
        {
            wheel.RotationDirection = -rotateDirection;
        }
    }

    void FixedUpdate ()
    {
        Vector3 moveForce = transform.forward;
        if (moveState == ShipMoveState.Slow)
        {
            moveForce *= slowMovePower;
        }
        else if (moveState == ShipMoveState.Fast)
        {
            moveForce *= fastMovePower;
        }

        Vector3 torque = transform.up;
        torque *= rotatePower * rotateDirection;

        shipRigidbody.AddForce (ShipMass * moveForce);
        shipRigidbody.AddTorque (ShipMass * torque);
    }

    public void IncreaseMoveSpeed ()
    {
        if (moveState == ShipMoveState.Fast)
        {
            return;
        }

        MoveState = (ShipMoveState)((int)moveState + 1);
    }

    public void DecreaseMoveSpeed ()
    {
        if (moveState == ShipMoveState.Stop)
        {
            return;
        }

        MoveState = (ShipMoveState)((int)moveState - 1);
    }

    public void Rotate (float direction)
    {
        rotateDirection = direction;
    }
}
