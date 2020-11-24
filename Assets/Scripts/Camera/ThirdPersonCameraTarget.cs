using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCameraTarget : MonoBehaviour
{
    [SerializeField]
    private float distance;
    [SerializeField]
    private float rotationSpeed;
    [SerializeField]
    private float minVerticalAngle;
    [SerializeField]
    private float maxVerticalAngle;

    public float Distance
    {
        get { return distance; }
    }
    public float RotationSpeed
    {
        get { return rotationSpeed; }
    }
    public float MinVerticalAngle
    {
        get { return minVerticalAngle; }
    }
    public float MaxVerticalAngle
    {
        get { return maxVerticalAngle; }
    }
}
