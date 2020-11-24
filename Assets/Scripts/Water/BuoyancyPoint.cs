using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct BuoyancyPoint
{
	[SerializeField]
	private Vector3 position;
	[SerializeField]
	private float radius;

	public Vector3 Position { get { return position; } }
	public float Radius { get { return radius; } }

	// distanceToSurface < 0 when point is above the sea surface
	public float GetImmersedVolume (float distanceToSurface)
	{
		if (Mathf.Abs (distanceToSurface) > radius && distanceToSurface < 0.0f)
		{
			return 0.0f;
		}

		float h = Mathf.Min (Mathf.Abs (distanceToSurface), 2.0f * radius);
		return (Mathf.PI * h * h) * (3.0f * radius - h) / 3.0f ;
	}
}
