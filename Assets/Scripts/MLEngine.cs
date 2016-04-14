using UnityEngine;
using System.Collections;
using System.Collections.Generic;


using System.Linq;

public class MLEngine : MonoBehaviour
{

	// Class to handle and coordinate all data and activity. All 'active' code is controlled here.

	GameObject MainCamera, CardboardMain, OculusMain;
	GameObject visualisationObject;
	GameObject title;

	Island currentIsland;
	ArrayList theIslands;

	MLSettings settings;
	//	MLUx ux;

	Material mat;
	//	MLDirector director;

	// ***************************************************************************************************
	// Unity start & update methods
	// ***************************************************************************************************

	void Start ()
	{
//		Application.setTargetFrameRate (60);
		Application.targetFrameRate = 60;
		title = GameObject.Find ("Title");
			
		settings = GameObject.Find ("Root").GetComponent <MLSettings> (); // get a reference to the settings
//		ux = GameObject.Find ("Root").GetComponent <MLUx> (); // get a reference to the interaction controller


//		visualisationObject = new GameObject ("MainVisualisationObject");
//		visualisationObject.transform.parent = GameObject.Find ("Root").transform;

		visualisationObject = GameObject.Find ("MainVisualisationObject");

//		director = new MLDirector ();
		// Create a container to hold our islands, which in turn will hold terrain, flock, cameras, etc.

		theIslands = new ArrayList ();
		currentIsland = spawnIsland (settings.initialVerticeCount, settings.initialAmplitude, settings.initialRoughness);
		theIslands.Add (currentIsland);

		// Get references to the project cameras.
		// Islands have 'camera' gameobjects. In the piece, we'll plot the data from those objects onto the actual project cameras: generic, cardboard, oculus.

		MainCamera = settings.MainCamera;
		CardboardMain = settings.CardboardMain;

		OculusMain = settings.OculusMain; 






		// 
		mat = Resources.Load ("Colour01") as Material;
		GameObject.Find ("Cube").GetComponent<Renderer> ().material = mat;

		goTo ();

	}


	// Update is called once per frame
	void Update ()
	{



		// Call counter script for onscreen titles
		if (titleTimer ()) {
			title.SetActive (false);
		}




//		if (! GameObject.Find ("OVRCameraRig").GetComponent<OVRManager > ().isUserPresent ) {
//
//			Debug.Log ("No oculus rift user present");
//
//		}


		// Plot 'current' camera from current island onto the project camera, depending on platform.

		switch (settings.thePlatform) {

		case "iOS":
			CardboardMain.transform.position = currentCamera.transform.position;
			CardboardMain.transform.rotation = currentCamera.transform.rotation;
			break;

		case "OSX":
			if (OVRManager.isHmdPresent) {
				OculusMain.transform.position = currentCamera.transform.position;
				OculusMain.transform.rotation = currentCamera.transform.rotation;

			} else {
//				Debug.Log ("No headset");
				MainCamera.transform.position = currentCamera.transform.position;
				MainCamera.transform.rotation = currentCamera.transform.rotation;
				title.transform.position = currentCamera.transform.position;
				title.transform.rotation = currentCamera.transform.rotation;

			}
			break;

		default:
			MainCamera.transform.position = currentCamera.transform.position;
			MainCamera.transform.rotation = currentCamera.transform.rotation;
			title.transform.position = currentCamera.transform.position;
			title.transform.rotation = currentCamera.transform.rotation;
			break;


		}


		switch (currentChapter) {

		case "none":


			break;

		case "islandLoading":
			currentGeometryPlayer.update ();


			break;

		default:

			break;


		}




		//		if (settings.directorOn) {
		//			// needs to move to MLEngine
		//
		//			if (Random.value < (1f / 180f)) {
		//				//				world.getCurrentIsland ().setCurrentCamera (Mathf.FloorToInt(Random.Range(0f,1.99999f)));
		//				theCamera = engine.getCurrentIsland ().setCurrentCamera (Mathf.FloorToInt (Random.Range (0f, 1.99999f)));
		//
		//			}
		//		} else {
		//			theCamera = engine.getCurrentIsland ().setCurrentCamera (settings.currentCameraNo);
		//		}


//		director.update ();




	}


	// ***************************************************************************************************
	// Island & buffer methods
	// ***************************************************************************************************


//	public void reSpawnCurrentIsland (int verticeCount, float amplitude, float roughness)
//	{
//		// Destroy current and span new island.
//		Destroy (currentIsland.getGameObject (), 0f);
//		currentIsland = spawnIsland (verticeCount, amplitude, roughness);
//		currentCamera = (currentIsland.getCamera (settings.currentCameraNo));
//
//	}

	public void reSpawnCurrentIsland (int verticeCount, float amplitude, float roughness)
	{
		// Destroy current and span new island.
		deleteCurrentBuffer ();
		addNewIsland (verticeCount, amplitude, roughness);

	}



	public void addNewIsland (int verticeCount, float amplitude, float roughness)
	{
		// Add a new island to the buffer, switch the view, drop cam
		currentIsland.setVisible (false);
		currentIsland = spawnIsland (verticeCount, amplitude, roughness);
		theIslands.Add (currentIsland);
		currentCamera = (currentIsland.getCamera (settings.currentCameraNo));

	}

	public void switchToIsland (int index)
	{
		// Switch to a buffered island, drop cam
		if (index < theIslands.Count) {
			currentIsland.setVisible (false);
			currentIsland = (Island)theIslands [index];
			currentIsland.setVisible (true);
			currentCamera = (currentIsland.getCamera (settings.currentCameraNo));

		}
	}

	Island spawnIsland (int verticeCount, float amplitude, float roughness)
	{
		// Spawn a new island and return a reference
		Island theIsland = new Island (GameObject.Find ("Root"), "Island 01");
		theIsland.setLineColor (settings.lineColor01);

		theIsland.spawnTerrain (settings.size, 4, amplitude, roughness);
		theIsland.spawnDelauney (verticeCount, settings.size);
		theIsland.spawnFlock ();
		theIsland.initialiseCameras ();
		theIsland.addDefaultCameras ();
		currentCamera = (theIsland.getCamera (settings.currentCameraNo));
			
		return theIsland;
	}



	public void deleteCurrentBuffer ()
	{
		// Delete the current island from the buffer and switch to 0
		Destroy (currentIsland.getGameObject (), 0f);
		theIslands.Remove (currentIsland);
		switchToIsland (0);
		currentCamera = (currentIsland.getCamera (settings.currentCameraNo));


	}

	// ***************************************************************************************************
	// Story / director methods
	// ***************************************************************************************************

	GameObject currentCamera;
	GeometryPlayer currentGeometryPlayer;


	string[] chapters = { "none", "islandLoading" };
	int currentChapterIndex = -1;
	string currentChapter;

	public void initChapter (int i)
	{
		if (i != currentChapterIndex) {



			if (i > chapters.Length)
				i = 0;



			string newChapter = chapters [i];


			switch (newChapter) {

			case "none":
				settings.UICanvas.SetActive (true);
				showTitle ("Now in", "Editor mode", 2f);
				break;

			case "islandLoading":
				settings.UICanvas.SetActive (false);
			// prepare for island creation animation
				Destroy (currentIsland.getGameObject (), 0f);
							
				theIslands.Remove (currentIsland);
		




			// Spawn a new island and return a reference
				Island theIsland = new Island (GameObject.Find ("Root"), "Island Animation");


				GameObject playerObject = new GameObject ("delauney playback");
				playerObject.transform.parent = theIsland.getGameObject().transform;

				currentGeometryPlayer = new GeometryPlayer (playerObject);
				//				currentGeometryPlayer = new GeometryPlayer (GameObject.Find ("GeometryPlayback"));



				theIsland.setLineColor (settings.lineColor01);

				theIsland.spawnTerrain (settings.size, 4, settings.initialAmplitude, settings.initialRoughness);
				theIsland.spawnDelauney (settings.initialVerticeCount, settings.size, currentGeometryPlayer);
				theIsland.initialiseCameras ();
				theIsland.addOrbitCamera ();
				settings.currentCameraNo = 0;
				currentCamera = (theIsland.getCamera (settings.currentCameraNo));

				currentIsland = theIsland;
				theIslands.Add (currentIsland);
				showTitle ("Part 1","Delauney", 5f);

				break;

			default:

				break;


			}
			currentChapterIndex = i;
			currentChapter = newChapter;
		}
	}

	public void exit ()
	{
		settings.chapter = 0;
		goTo ();

	}

	public void goTo ()
	{
		initChapter (settings.chapter);
		currentCamera = (currentIsland.getCamera (settings.currentCameraNo));

	}

	public void nextChapter ()
	{
		settings.chapter = (settings.chapter + 1) % chapters.Length;
		goTo ();

	}

	public void previousChapter ()
	{
		settings.chapter = (settings.chapter - 1) % chapters.Length;
		goTo ();

	}


	private float c;

	private bool titleTimer ()
	{
		bool returnValue;
		returnValue = false;

		if (c > 0) {
			c = c - Time.deltaTime;
			if (c < 0) {
				returnValue = true;
			}
		} 
		return returnValue;
	}

	public void showTitle (string sub,string main, float theTime)
	{
		title.SetActive (true);
		title.transform.FindChild ("Main").GetComponent<TextMesh> ().text = main;
		title.transform.FindChild ("Sub").GetComponent<TextMesh> ().text = sub;
		c = theTime;

	}



	// ***************************************************************************************************
	// Public access methods
	// ***************************************************************************************************
	public GameObject getCurrentCamera ()
	{
		return		currentCamera;
	}

	public int getCurrentBufferSize ()
	{
		return theIslands.Count;
	}

	public int getCurrentBuffer ()
	{
		return theIslands.IndexOf (currentIsland);
	}

	public GameObject getVisualDebugObject ()
	{
		return visualisationObject;
	}


	public Island getCurrentIsland ()
	{
		return currentIsland;
	}

	public int getNumberOfIslands ()
	{
		return theIslands.Count;
	}

}
