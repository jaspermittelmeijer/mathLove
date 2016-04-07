using UnityEngine;
using System.Collections;

public class ExtendedMesh {

	public Mesh theMesh;
	float duration;

//	public ExtendedMesh (Mesh passMesh, float passDuration){
//		theMesh = passMesh;
//		duration = passDuration;
//
//	}

	public ExtendedMesh (UnityEngine.Vector3[] passVertices, int[] passTriangles, float passDuration){
		
		theMesh = new Mesh ();
		theMesh.vertices = passVertices;
		theMesh.triangles = passTriangles;
		theMesh.RecalculateNormals ();

		duration = passDuration;


	}

	public Mesh getMesh () {
		return theMesh;
	}

	public float getDuration (){
		return duration;
	}

}
