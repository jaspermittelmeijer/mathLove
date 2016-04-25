using UnityEngine;
using System.Collections;
using System.Linq;

public class MLMesh
{

	// A container for a



	public Mesh theMesh;
	float duration;


	public MLMesh (UnityEngine.Vector3[] passVertices, int[] passTriangles, float passDuration)
	{

		theMesh = new Mesh ();
		theMesh.vertices = passVertices;
		theMesh.triangles = passTriangles;
		theMesh.RecalculateNormals ();

		theMesh = addBackSide (theMesh);

		duration = passDuration;


	}

	Mesh addBackSide (Mesh inputMesh)
	{
		CombineInstance[] combine = new CombineInstance[2];
		Mesh backSideMesh = new Mesh ();
		backSideMesh.vertices = inputMesh.vertices;

		Vector3[] normals = inputMesh.normals;
		for (int i = 0; i < normals.Length; i++)
			normals [i] = -normals [i];
		backSideMesh.normals = normals;
		backSideMesh.triangles = inputMesh.triangles.Reverse ().ToArray ();

		combine [0].mesh = inputMesh;
		combine [1].mesh = backSideMesh;

		Mesh combinedMesh = new Mesh ();
		combinedMesh.CombineMeshes (combine, true, false);
		return combinedMesh;
	}



	public Mesh getMesh ()
	{
		return theMesh;
	}

	public float getDuration ()
	{
		return duration;
	}

}




