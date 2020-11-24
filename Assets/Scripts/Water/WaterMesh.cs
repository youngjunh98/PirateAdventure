using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterMesh : MonoBehaviour
{
	public Material material;
	public int tileSize;
	public float tileScale;

	void Awake ()
	{
		MeshFilter meshFilter = gameObject.AddComponent<MeshFilter> ();
		meshFilter.mesh = CreateSeaTile (tileSize, tileScale, transform.position);

		MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer> ();
		meshRenderer.material = material;
		meshRenderer.material.SetFloat ("_TileSize", tileSize * tileScale);
	}

	private Mesh CreateSeaTile (int size, float scale, Vector3 position)
	{
		int vertexCount = (size + 1) * (size + 1);
		Vector3[] vertices = new Vector3[vertexCount];
		Vector3[] normals = new Vector3[vertexCount];

		Vector3 offset = position;
		offset -= 0.5f * scale * size * Vector3.right;
		offset -= 0.5f * scale * size * Vector3.forward;

		for (int z = 0; z <= size; z++)
		{
			for (int x = 0; x <= size; x++)
			{
				Vector3 vertex = scale * new Vector3 (x, 0.0f, z);
				vertices[(size + 1) * z + x] = vertex + offset;
				normals[(size + 1) * z + x] = Vector3.up;
			}
		}

		int triangleIndexCount = 6 * size * size;
		int[] triangleIndices = new int[triangleIndexCount];

		for (int i = 0; i < size; i++)
		{
			for (int j = 0; j < size; j++)
			{
				int leftBottomVertexIndex = i * (size + 1) + j;
				int rightBottomVertexIndex = leftBottomVertexIndex + 1;
				int leftTopVertexIndex = leftBottomVertexIndex + (size + 1);
				int rightTopVertexIndex= leftTopVertexIndex + 1;
				
				int startIndex = (6 * size * i) + (6 * j);

				triangleIndices[startIndex + 0] = leftBottomVertexIndex;
				triangleIndices[startIndex + 1] = leftTopVertexIndex;
				triangleIndices[startIndex + 2] = rightBottomVertexIndex;

				triangleIndices[startIndex + 3] = rightBottomVertexIndex;
				triangleIndices[startIndex + 4] = leftTopVertexIndex;
				triangleIndices[startIndex + 5] = rightTopVertexIndex;
			}
		}

		Mesh mesh = new Mesh ();
		mesh.vertices = vertices;
		mesh.normals = normals;
		mesh.triangles = triangleIndices;

		return mesh;
	}
}
