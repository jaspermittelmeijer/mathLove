using UnityEngine;
using System.Collections;
using System.Collections.Generic;


using System.Linq;

public class World : MonoBehaviour
{

	// Class to handle and coordinate all data and activity. All 'active' code is controlled here.

	GameObject MainCamera, CardboardMain, OculusMain;
	GameObject visualisationObject;

	Island currentIsland;
	ArrayList theIslands;

	Settings settings;
	Interaction interaction;

	Material mat;




	void Start ()
	{
//		Application.setTargetFrameRate (60);
		Application.targetFrameRate = 60;

			
		settings = GameObject.Find ("Root").GetComponent <Settings> (); // get a reference to the settings
		interaction = GameObject.Find ("Root").GetComponent <Interaction> (); // get a reference to the interaction controller


//		visualisationObject = new GameObject ("MainVisualisationObject");
//		visualisationObject.transform.parent = GameObject.Find ("Root").transform;

		visualisationObject = GameObject.Find ("MainVisualisationObject");


		// Create a container to hold our islands, which in turn will hold terrain, flock, cameras, etc.

		theIslands = new ArrayList ();
		currentIsland = spawnIsland (settings.initialVerticeCount, settings.initialAmplitude, settings.initialRoughness);
		theIslands.Add (currentIsland);

		// Get references to the project cameras.
		// Islands have 'camera' gameobjects. In the piece, we'll plot the data from those objects onto the actual project cameras: generic, cardboard, oculus.

		MainCamera = settings.MainCamera;
		CardboardMain = settings.CardboardMain;
		OculusMain = settings.OculusMain; // to be implemented

		// 
		mat = Resources.Load ("Colour01") as Material;
		GameObject.Find ("Cube").GetComponent<Renderer> ().material = mat;

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

	public void reSpawnCurrentIsland (int verticeCount, float amplitude, float roughness)
	{
		// (currently not directly in use). Destroy current and span new island.
		Destroy (currentIsland.getGameObject (), 0f);
		currentIsland = spawnIsland (verticeCount, amplitude, roughness);
	}

	public void addNewIsland (int verticeCount, float amplitude, float roughness)
	{
		// Add a new island to the buffer, switch the view, drop cam
		currentIsland.setVisible (false);
		currentIsland = spawnIsland (verticeCount, amplitude, roughness);
		theIslands.Add (currentIsland);
	}

	public void switchToIsland (int index)
	{
		// Switch to a buffered island, drop cam
		if (index < theIslands.Count) {
			currentIsland.setVisible (false);
			currentIsland = (Island)theIslands [index];
			currentIsland.setVisible (true);
		}
	}


	public Island spawnIsland (int verticeCount, float amplitude, float roughness)
	{
		// Spawn a new island and return a reference
		Island theIsland = new Island (GameObject.Find ("Root"), "Island 01");
		theIsland.setLineColor (settings.lineColor01);

		theIsland.spawnTerrain (settings.size, 4, amplitude, roughness);
		theIsland.spawnDelauney (verticeCount, settings.size);
		theIsland.spawnFlock ();
		theIsland.initialiseCameras ();

		return theIsland;
	}

	public int getCurrentBuffer ()
	{
		return theIslands.IndexOf (currentIsland);
	}

	public void deleteCurrentBuffer ()
	{
		// Delete the current island from the buffer and switch to 0
		Destroy (currentIsland.getGameObject (), 0f);
		theIslands.Remove (currentIsland);
		switchToIsland (0);

	}

	public int getCurrentBufferSize ()
	{
		return theIslands.Count;
	}


	// Update is called once per frame
	void Update ()
	{

		// Plot 'current' camera from current island onto the project camera, depending on platform.

		switch (settings.thePlatform) {

		case "iOS":
			CardboardMain.transform.position = currentIsland.getCurrentCamera ().transform.position;
			CardboardMain.transform.rotation = currentIsland.getCurrentCamera ().transform.rotation;
			break;

		case "OSX":
			OculusMain.transform.position = currentIsland.getCurrentCamera ().transform.position;
			OculusMain.transform.rotation = currentIsland.getCurrentCamera ().transform.rotation;
			break;

		default:
			MainCamera.transform.position = currentIsland.getCurrentCamera ().transform.position;
			MainCamera.transform.rotation = currentIsland.getCurrentCamera ().transform.rotation;
			break;


		}

		/*
		if (settings.thePlatform == "iOS") {
			CardboardMain.transform.position = currentIsland.getCurrentCamera ().transform.position;
			CardboardMain.transform.rotation = currentIsland.getCurrentCamera ().transform.rotation;

		} else {

			MainCamera.transform.position = currentIsland.getCurrentCamera ().transform.position;
			MainCamera.transform.rotation = currentIsland.getCurrentCamera ().transform.rotation;

		}
		*/




	}


}
