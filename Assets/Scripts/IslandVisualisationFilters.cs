using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using System.Linq;


public enum ISLANDLOOK
{
BASIC,
	TRANSPARENT}
;


public struct IslandVisualisationFilters
{

	Color BASIC_linecolour;

	public IslandVisualisationFilters (int n)
	{
		BASIC_linecolour = GameObject.Find ("Root").GetComponent <MLSettings> ().lineColor01;
	}



	public GameObject parseRawData (ISLANDLOOK _look, GameObject _parent, Vector3[] _vertices, int[] _triangles)
	{
		GameObject workingObject = new GameObject ("Geometry");
		workingObject.transform.parent = _parent.transform;

		switch (_look) {

		case ISLANDLOOK.BASIC:
			
			Mesh theMesh = new Mesh ();
			theMesh.vertices = _vertices;
			theMesh.triangles = _triangles;
			theMesh.RecalculateNormals ();

			theMesh = addBackSide (theMesh);

			workingObject.AddComponent<MeshFilter> ();
			workingObject.AddComponent<MeshRenderer> ();
			workingObject.AddComponent<CustomRender> ();

			workingObject.GetComponent<MeshFilter> ().mesh = theMesh;
			workingObject.GetComponent <CustomRender> ().CreateLinesFromMesh ();

			workingObject.GetComponent<CustomRender> ().passColor (BASIC_linecolour);

			workingObject.GetComponent<Renderer> ().material = Resources.Load ("Default") as Material;
			workingObject.GetComponent<Renderer> ().useLightProbes = false;
			workingObject.GetComponent<Renderer> ().reflectionProbeUsage = ReflectionProbeUsage.Off;

		//		workingObject.GetComponent<Renderer> ().shadowCastingMode = ShadowCastingMode.TwoSided;
			workingObject.GetComponent<Renderer> ().receiveShadows = true;
			workingObject.SetActive (false);

			return workingObject;

			break;

		default:
			return workingObject;
			break;

		}

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


}
