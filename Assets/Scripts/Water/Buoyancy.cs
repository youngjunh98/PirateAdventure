using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Rigidbody))]
public class Buoyancy : MonoBehaviour
{
	[SerializeField]
	private float waterDensity;
	[SerializeField]
	private float dragCoefficient;
	[SerializeField]
	private BuoyancyPoint[] buoyancyPoints;

	private Rigidbody rigidbody;
	private Water m_water;

	void Awake ()
	{
		rigidbody = GetComponent<Rigidbody> ();
		m_water = GameObject.Find ("Water").GetComponent<Water> ();
	}

	void Start ()
	{
		//Vector3 position = transform.position;
		//float seaSurfaceHeightSum = 0.0f;

		//for (int i = 0; i < buoyancyPoints.Length; i++)
		//{
		//	Vector3 pointPosition = transform.TransformPoint (buoyancyPoints[i].Position);
		//	seaSurfaceHeightSum += m_water.GetSurfaceHeight (pointPosition);
		//}
		
		//position.y = seaSurfaceHeightSum / buoyancyPoints.Length;
		//transform.position = position;
	}

	void FixedUpdate ()
	{
		if (buoyancyPoints == null)
		{
			return;
		}

		for (int i = 0; i < buoyancyPoints.Length; i++)
		{			
			Vector3 pointPosition = transform.TransformPoint (buoyancyPoints[i].Position);
			float surfaceHeight = m_water.GetSurfaceHeight (pointPosition);
			float distanceToSurface = surfaceHeight - pointPosition.y;

			float immersedVolume = buoyancyPoints[i].GetImmersedVolume (distanceToSurface);
			float gravity = Mathf.Abs (Physics.gravity.y);
			Vector3 buoyancyFore = waterDensity * gravity * Mathf.Max (distanceToSurface, 0.0f) * immersedVolume * Vector3.up;

			Vector3 velocity = rigidbody.GetPointVelocity (pointPosition);
			Vector3 dragForce = dragCoefficient * waterDensity * 0.5f * -(velocity.normalized) * velocity.sqrMagnitude * immersedVolume;

			if (dragForce.sqrMagnitude < 0.1f)
			{
				dragForce = Vector3.zero;
			}
			
			rigidbody.AddForceAtPosition (buoyancyFore + dragForce, pointPosition, ForceMode.Force);
			rigidbody.AddForceAtPosition (Physics.gravity, pointPosition, ForceMode.Acceleration);
		}
	}

	void OnDrawGizmos ()
	{
		for (int i = 0; i < buoyancyPoints.Length; i++)
		{
			Vector3 worldPoint = transform.TransformPoint (buoyancyPoints[i].Position);

			Gizmos.color = Color.white;
			Gizmos.DrawWireSphere (worldPoint, buoyancyPoints[i].Radius);
		}
	}
}
