using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct WaveData
{
	[SerializeField]
	private Vector2 waveDirection;
	[SerializeField]
	private float waveAmplitude;
	[SerializeField]
	private float waveLength;
	[SerializeField]
	private float waveSpeed;
	[SerializeField, Range (0.0f, 1.0f)]
	private float waveSteepness;

	public Vector2 Direction { get { return waveDirection.normalized; } }
	public float Amplitude { get { return waveAmplitude; } }
	public float Length { get { return waveLength; } }
	public float Speed { get { return waveSpeed; } }
	public float Steepness { get { return waveSteepness; } }
	public float Frequency { get { return 2.0f / waveLength; } }
	public float Phase { get { return waveSpeed * Frequency; } }
}

public class Water : MonoBehaviour
{
	[SerializeField]
	private Gradient m_absorptionColorRamp;
	[SerializeField]
	private Gradient m_scatteringColorRamp;
    [SerializeField]
    private WaveData[] m_waveDataArray;

	private Material m_material;
	private Texture2D m_colorRampTexture;

    private void Awake ()
    {
		m_material = GetComponent<Renderer> ().material;
    }

    private void Start ()
	{
		UpdateColorRampTexture ();
		UpdateWaterMaterial ();
	}

    private void Update ()
    {
		UpdateColorRampTexture ();
		UpdateWaterMaterial ();

		//Vector3 camPosition = Camera.main.transform.position;
		//transform.position = new Vector3 (camPosition.x, transform.position.y, camPosition.z);
	}

    private void UpdateWaterMaterial ()
	{
		if (!m_material)
        {
			return;
        }

		m_material.SetTexture ("_ColorRampTexture", m_colorRampTexture);

		var waveArray = new Vector4[2];
		var waveDirectionArray = new float[2 * 2];

		for (int i = 0; i < 2; i++)
        {
			waveArray[i].x = m_waveDataArray[i].Amplitude;
			waveArray[i].y = m_waveDataArray[i].Length;
			waveArray[i].z = m_waveDataArray[i].Speed;
			waveArray[i].w = m_waveDataArray[i].Steepness;

			waveDirectionArray[i * 2] = m_waveDataArray[i].Direction.x;
			waveDirectionArray[i * 2 + 1] = m_waveDataArray[i].Direction.y;
		}

		m_material.SetVectorArray ("_Wave", waveArray);
		m_material.SetFloatArray ("_WaveDirection", waveDirectionArray);
	}

	private void UpdateColorRampTexture ()
    {
		const int textureWidth = 128;

		if (!m_colorRampTexture)
        {
			m_colorRampTexture = new Texture2D (textureWidth, 2);
			m_colorRampTexture.wrapMode = TextureWrapMode.Clamp;
		}

		var pixels = new Color[textureWidth * 2];

		for (int i = 0; i < textureWidth; i++)
        {
			pixels[i] = m_absorptionColorRamp.Evaluate ((float) i / textureWidth);
			pixels[i + textureWidth] = m_scatteringColorRamp.Evaluate ((float) i / textureWidth);
		}

		m_colorRampTexture.SetPixels (pixels);
		m_colorRampTexture.Apply ();
	}

	public float GetSurfaceHeight (Vector3 position)
	{
		float surfaceHeight = transform.position.y;
		float heightOffset = 0.0f;

		for (int i = 0; i < m_waveDataArray.Length; i++)
		{
			float waveHeight = ApplyWave (position, m_waveDataArray[i], m_waveDataArray.Length);
			heightOffset += waveHeight;
		}

		return surfaceHeight + heightOffset;
	}

    private float ApplyWave (Vector3 position, WaveData wave, int waveCount)
    {
        float waveFrequency = wave.Frequency / 50.0f;
		float wavePhase = wave.Phase / 50.0f;
		float waveSteepness = wave.Steepness / (waveFrequency * wave.Amplitude * waveCount);

		float dot = Vector2.Dot (wave.Direction, new Vector2 (position.x, position.z));

		return wave.Amplitude * Mathf.Sin (waveFrequency * dot + wavePhase * Time.time);
    }
}
