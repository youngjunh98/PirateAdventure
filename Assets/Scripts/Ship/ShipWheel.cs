using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipWheel : MonoBehaviour
{
    [SerializeField]
    private Transform wheelTransform;
    [SerializeField]
    private float rotationSpeed;
    [SerializeField]
    private float releaseSpeed;
    [SerializeField]
    private float maxLoop;

    private float rotation;
    private float rotationDirection;

    public float RotationDirection
    {
        set { rotationDirection = value; }
    }
    
    void Start ()
    {
        rotation = 0.0f;
        rotationDirection = 0.0f;
    }

    void Update ()
    {
        bool isRotating = Mathf.Abs (rotationDirection) > float.Epsilon;
        float speed = isRotating ? rotationSpeed : releaseSpeed;
        float targetRotation = isRotating ? rotation + speed * rotationDirection : 0.0f;

        rotation = Mathf.MoveTowards (rotation, targetRotation, speed * Time.deltaTime);
        rotation = Mathf.Clamp (rotation, -maxLoop * 360.0f, maxLoop * 360.0f);

        wheelTransform.localRotation = Quaternion.AngleAxis (rotation, Vector3.forward);
    }
}
