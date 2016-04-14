using System;
using UnityEngine;
using System.Linq;
using System.Collections;

public class Island
{
	// Class to hold a complete island
	// Including terraindata, delauney, flock, cameras

	RandomTerrain iTerrain;
	Delauney iDelauney;
	Material iMaterial01;
	GameObject iSelf, iDelauneyObject, iTerrainObject, iFlock;
	Color iLineColor;
	Mesh workingMesh;
	GameObject localVisualisationObject;
	ArrayList cameras;
	MLSettings settings;

	public Island (GameObject _parentObject, String _myName)
	{
		settings = GameObject.Find ("Root").GetComponent <MLSettings> (); // get a reference to the settings

		// basic initialisation
		initialiseIsland (_parentObject, _myName);
		setDefaultMaterials ();
	}


	// ***************************************************************************************************
	// Camera methods
	// ***************************************************************************************************

	public void initialiseCameras ()
	{
		cameras = new ArrayList ();
//		cameras.Add (addOrbitCamera ("Orbitcam"));
//		cameras.Add (addTargetCamera ("TargetCam01", 0));
//		cameras.Add (addStaticCamera ("StaticCam02"));
//		cameras.Add (addStaticCamera ("StaticCam03"));
	}

	public void addOrbitCamera ()
	{
		cameras.Add (addOrbitCamera ("Orbitcam"));
	
	}

	public void addDefaultCameras ()
	{
		
		cameras.Add (addOrbitCamera ("Orbitcam"));
		cameras.Add (addTargetCamera ("TargetCam01", 0));
		cameras.Add (addStaticCamera ("StaticCam02"));
		cameras.Add (addStaticCamera ("StaticCam03"));
	}


	public GameObject getCamera (int i)
	{
		return (GameObject)cameras [i];

	}

	public int getCameraCount ()
	{
		return cameras.Count;
	}

	GameObject addOrbitCamera (String _name)
	{
		GameObject workingObject = new GameObject (_name + "_anchor");
		workingObject.transform.parent = iSelf.transform;
		workingObject.transform.position = new Vector3 (0.5f * settings.size, 0.0f * settings.size, 0.5f * settings.size);

		GameObject newCamera = new GameObject (_name);
		newCamera.transform.parent = workingObject.transform;

		Vector3 cameraPosition = new Vector3 (1.5f * settings.size, 0.1f * settings.size, 1.5f * settings.size);

		newCamera.transform.position = cameraPosition;
		newCamera.transform.localRotation = Quaternion.LookRotation (workingObject.transform.position - newCamera.transform.position, Vector3.up);

		workingObject.AddComponent <OrbitCam> ();


		return newCamera;
	}

	GameObject addStaticCamera (String _name)
	{
		// drop a new camera at a random point
		GameObject newCamera = new GameObject (_name);
		newCamera.transform.parent = iSelf.transform;

		Vector3 cameraPosition = new Vector3 (UnityEngine.Random.Range (0.0f, settings.size), 0.0f, UnityEngine.Random.Range (0.0f, settings.size));

//		cameraPosition.y = getHeight (cameraPosition.x, cameraPosition.z) + 0.5f * settings.initialAmplitude;
		cameraPosition.y = getHeight (cameraPosition.x, cameraPosition.z) + 0.1f * settings.initialAmplitude;

		newCamera.transform.position = cameraPosition;
		newCamera.transform.localRotation = Quaternion.LookRotation (new Vector3 (settings.size * .25f, 0f, settings.size * .25f) - newCamera.transform.position, Vector3.up);

		return newCamera;
	}

	public GameObject addTargetCamera (String _name, int _targetBoid)
	{
		GameObject newCamera = new GameObject (_name);
		newCamera.transform.parent = iSelf.transform;
		GameObject theTarget = iFlock.GetComponent<Flock> ().getBoid (_targetBoid);

		FollowCam cameraControl = newCamera.AddComponent <FollowCam> ();

		cameraControl.setTarget (theTarget);

		cameraControl.setTargetDebug (VisualisationObject.newNull (theTarget.transform.position, 0.25f, localVisualisationObject));

		return newCamera;
	}

	// ***************************************************************************************************
	// Rendering methods
	// ***************************************************************************************************

	private void setDefaultMaterials ()
	{
		setMaterial (Resources.Load ("Default") as Material);
		iLineColor = Color.black; // setting a default linecolor
	}

	public void setVisible (bool visible)
	{
		iSelf.SetActive (visible);
	}

	public void setMaterial (Material _mat)
	{
		iMaterial01 = _mat;
	}

	public void setLineColor (Color _color)
	{
		iLineColor = _color;
	}


	// ***************************************************************************************************
	// Island creation methods
	// ***************************************************************************************************

	private void initialiseIsland (GameObject _parent, String _name)
	{
		iSelf = new GameObject (_name);
		iSelf.transform.parent = _parent.transform;

		localVisualisationObject = new GameObject ("LocalVisualisationObject");
		localVisualisationObject.transform.parent = iSelf.transform;

	}

	public void spawnFlock ()
	{
		iFlock = new GameObject ("iFlock");
		iFlock.transform.parent = iSelf.transform;
		iFlock.AddComponent <Flock> ().createFlock ();
	}

	public void spawnTerrain (float _size, int _iterations, float _amp, float _roughness)
	{

		iTerrain = new RandomTerrain (_size, _iterations, _amp, _roughness);
		iTerrain.setVisualisationTarget (localVisualisationObject);
		iTerrain.spawn ();


		iTerrainObject = new GameObject ("iTerrain");
		iTerrainObject.transform.parent = iSelf.transform;
		iTerrainObject.AddComponent<MeshFilter> ();
		iTerrainObject.AddComponent<MeshRenderer> ();
		iTerrainObject.AddComponent<CustomRender> ();

		workingMesh = new Mesh ();
		iTerrainObject.GetComponent<MeshFilter> ().mesh = workingMesh;

		workingMesh.vertices = iTerrain.getVertices ();
		workingMesh.triangles = iTerrain.getTriangles ();
		workingMesh.RecalculateNormals ();
		iTerrainObject.GetComponent <CustomRender> ().CreateLinesFromMesh ();

		iTerrainObject.GetComponent<Renderer> ().material = iMaterial01;
		iTerrainObject.GetComponent<CustomRender> ().passColor (iLineColor);
		iTerrainObject.SetActive (false);

	}

	public void spawnDelauney (int vertices, float size)
	{
		iDelauney = new Delauney ();
		iDelauney.createDelauney (vertices, size, iTerrain);
		iDelauneyObject = new GameObject ("iDelauney");
		iDelauneyObject.transform.parent = iSelf.transform;

		iDelauneyObject.AddComponent<MeshFilter> ();
		iDelauneyObject.AddComponent<MeshRenderer> ();
		iDelauneyObject.AddComponent<CustomRender> ();

		workingMesh = new Mesh ();
		workingMesh.vertices = iDelauney.getUniqueVertices ();
		workingMesh.triangles = iDelauney.getUniqueTriangles ();
		workingMesh.RecalculateNormals ();

		iDelauneyObject.GetComponent<MeshFilter> ().mesh = addBackSide (workingMesh);

			
		iDelauneyObject.GetComponent<CustomRender> ().passColor (iLineColor);
		iDelauneyObject.GetComponent <CustomRender> ().CreateLinesFromMesh ();

		iDelauneyObject.GetComponent<Renderer> ().material = iMaterial01;

		iDelauneyObject.AddComponent <MeshCollider> ();

		iDelauneyObject.SetActive (true);

	}
	/*
	public void spawnDelauneyLegacy (int vertices, float size)
	{
		iDelauney = new Delauney ();
		iDelauney.createDelauney (vertices, size, iTerrain);
		iDelauneyObject = new GameObject ("iDelauney");
		iDelauneyObject.transform.parent = iSelf.transform;

		iDelauneyObject.AddComponent<MeshFilter> ();
		iDelauneyObject.AddComponent<MeshRenderer> ();
		iDelauneyObject.AddComponent<CustomRender> ();

		workingMesh = new Mesh ();
		Mesh workingMesh2 = new Mesh ();

		CombineInstance[] combine = new CombineInstance[2];

		workingMesh.vertices = iDelauney.getUniqueVertices ();
		workingMesh.triangles = iDelauney.getUniqueTriangles ();
		workingMesh.RecalculateNormals ();

		combine [0].mesh  = workingMesh;

		workingMesh2.vertices = iDelauney.getUniqueVertices ();
		workingMesh2.triangles = iDelauney.getUniqueTriangles ();
		workingMesh2.RecalculateNormals ();
		workingMesh2.triangles = workingMesh2.triangles.Reverse ().ToArray ();

		combine [1].mesh = workingMesh2;

		iDelauneyObject.GetComponent<MeshFilter> ().mesh = new Mesh();
		iDelauneyObject.GetComponent<MeshFilter> ().mesh.CombineMeshes(combine,true,false);

		iDelauneyObject.GetComponent<CustomRender> ().passColor (iLineColor);
		iDelauneyObject.GetComponent <CustomRender> ().CreateLinesFromMesh ();

		iDelauneyObject.GetComponent<Renderer> ().material = iMaterial01;

		iDelauneyObject.AddComponent <MeshCollider> ();

		iDelauneyObject.SetActive (true);

	}
*/


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


	public void spawnDelauney (int vertices, float size, GeometryPlayer targetGeometryPlayer)
	{

		iDelauney = new Delauney ();
		iDelauney.createDelauney (vertices, size, iTerrain, targetGeometryPlayer);
		iDelauneyObject = new GameObject ("iDelauney");
		iDelauneyObject.transform.parent = iSelf.transform;

		iDelauneyObject.AddComponent<MeshFilter> ();
		iDelauneyObject.AddComponent<MeshRenderer> ();
		iDelauneyObject.AddComponent<CustomRender> ();

		workingMesh = new Mesh ();
		workingMesh.vertices = iDelauney.getUniqueVertices ();
		workingMesh.triangles = iDelauney.getUniqueTriangles ();
		workingMesh.RecalculateNormals ();

		iDelauneyObject.GetComponent<MeshFilter> ().mesh = addBackSide (workingMesh);


		iDelauneyObject.GetComponent<CustomRender> ().passColor (iLineColor);
		iDelauneyObject.GetComponent <CustomRender> ().CreateLinesFromMesh ();

		iDelauneyObject.GetComponent<Renderer> ().material = iMaterial01;

		iDelauneyObject.AddComponent <MeshCollider> ();

		iDelauneyObject.SetActive (false);




//		

	}







	/*
	private void spawnBackfaceObject ()
	{
		GameObject workingObject = new GameObject ("iDelauney Backfaces");
		workingObject.transform.parent = iSelf.transform;

		workingObject.AddComponent<MeshFilter> ();
		workingObject.AddComponent<MeshRenderer> ();


		workingMesh = new Mesh ();
		workingObject.GetComponent<MeshFilter> ().mesh = workingMesh;

		workingMesh.vertices = iDelauney.getUniqueVertices ();
		workingMesh.triangles = iDelauney.getUniqueTriangles ();
		//		mesh.triangles = mesh.triangles.Reverse ();
		workingMesh.RecalculateNormals ();

		workingMesh.triangles = workingMesh.triangles.Reverse ().ToArray ();

		workingObject.GetComponent<Renderer> ().material = iMaterial01;
		workingObject.GetComponent<Renderer> ().receiveShadows = false;

		workingObject.SetActive (true);


	}
*/

	// ***************************************************************************************************
	// Public access methods
	// ***************************************************************************************************


	public float getHeight (float i, float j)
	{
		float height = iTerrain.getHeight (i, j);

		RaycastHit theHit;
		if (Physics.Raycast (new Vector3 (i, 15f, j), Vector3.down, out theHit, 100f)) {
			height = 15f - theHit.distance;
		}


		return (height);
	}

	public GameObject getGameObject ()
	{
		return (iSelf);
	}

	public Delauney getDelauney ()
	{
		return (iDelauney);
	}



}


