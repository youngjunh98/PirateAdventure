using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField]
    private Transform m_targetTransform;
    [SerializeField]
    private float m_followSpeed = 30.0f;
    [SerializeField]
    private float m_brakeDistance = 1.0f;
    [SerializeField]
    private float m_rotateSpeed = 720.0f;
    [SerializeField]
    private Vector3 m_minAngles = new Vector3 (0.001f, 0.0f, 0.0f);
    [SerializeField]
    private Vector3 m_maxAngles = new Vector3 (179.999f, 360.0f, 360.0f);
    [SerializeField]
    private float m_zoomSpeed = 10.0f;
    [SerializeField]
    private float m_minDistance = 0.0001f;
    [SerializeField]
    private float m_maxDistance = 100.0f;

    private Vector3 m_updatedTargetPosition;
    private Vector3 m_angles;
    private float m_distance;

    public Transform TargetTransform
    {
        get { return m_targetTransform; }
        set
        {
            m_targetTransform = value;

            if (m_targetTransform)
            {
                m_updatedTargetPosition = m_targetTransform.position;
            }
        }
    }

    public float PolarAngle
    {
        get { return m_angles.x; }
        set { m_angles.x = Mathf.Clamp (ClampAngle360 (value), m_minAngles.x, m_maxAngles.x); }
    }

    public float AzimuthalAngle
    {
        get { return m_angles.y; }
        set { m_angles.y = Mathf.Clamp (ClampAngle360 (value), m_minAngles.y, m_maxAngles.y); }
    }

    public float Roll
    {
        get { return m_angles.z; }
        set { m_angles.z = Mathf.Clamp (ClampAngle360 (value), m_minAngles.z, m_maxAngles.z); }
    }

    public float Distance
    {
        get { return m_distance; }
        set { m_distance = Mathf.Clamp (value, m_minDistance, m_maxDistance); }
    }

    private void Awake ()
    {
        TargetTransform = m_targetTransform;
    }

    private void LateUpdate ()
    {
        if (!m_targetTransform)
        {
            return;
        }

        Vector3 targetPosition = TargetTransform.position;
        float updateRemaining = (targetPosition - m_updatedTargetPosition).magnitude;

        if (updateRemaining < 0.01f)
        {
            m_updatedTargetPosition = targetPosition;
        }
        else
        {
            float speedInterpolation = Mathf.Clamp01 (updateRemaining / m_brakeDistance);
            float speed = Mathf.Lerp (0.0f, m_followSpeed, speedInterpolation);
            float updateStep = speed * Time.deltaTime;
            m_updatedTargetPosition = Vector3.MoveTowards (m_updatedTargetPosition, targetPosition, updateStep);
        }

        Vector3 positionOffset = Quaternion.Euler (PolarAngle, AzimuthalAngle, 0.0f) * (Vector3.up * m_distance);
        transform.position = m_updatedTargetPosition + positionOffset;

        Quaternion lookRotation = Quaternion.LookRotation (m_updatedTargetPosition - transform.position);
        Quaternion rollRotation = Quaternion.AngleAxis (m_angles.z, Vector3.forward);
        transform.rotation = lookRotation * rollRotation;
    }

    public void Rotate (float horizontal, float vertical, float rolling)
    {
        Vector3 normalized = new Vector3 (vertical, horizontal, rolling).normalized;
        float speed = m_rotateSpeed * Time.deltaTime;

        PolarAngle += normalized.x * speed;
        AzimuthalAngle += normalized.y * speed;
        Roll += normalized.z * speed;
    }

    public void Zoom (float direciton)
    {
        if (direciton != 0.0f)
        {
            direciton = Mathf.Sign (direciton);
        }

        Distance += direciton * m_zoomSpeed * Time.deltaTime;
    }

    private float ClampAngle360 (float angle)
    {
        float clamped = angle % 360.0f;

        if (clamped < 0.0f)
        {
            clamped += 360.0f;
        }

        return clamped;
    }

    private float ClampAngleSigned180 (float angle)
    {
        float clamped = ClampAngle360 (angle);

        if (clamped > 180.0f)
        {
            clamped -= 180.0f;
        }

        return clamped;
    }
}
