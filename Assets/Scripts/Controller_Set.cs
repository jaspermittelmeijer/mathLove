using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using System.Linq;

/*
// An experiment for async operation
public delegate void AsyncTest (int i);




// A general delegate to for classes to handle changes in the engine
public delegate void ChangedEventHandler (object sender, EngineChangedEventArgs e);

// Extend eventargs to hold our event info.
public class EngineChangedEventArgs : EventArgs
{
	public string thisHappened;
	public int newChapter;

	public EngineChangedEventArgs () : base ()
	{ // extend the contstructor as well
		
		// Fill the args with default values to prevent having to check for nulls. 
		thisHappened = "void";
		newChapter = 0;

	}
}
*/

public class Controller_Set : MonoBehaviour
{

	// Class to handle and coordinate all data and activity. All 'active' code is controlled here.

	//	GameObject MainCamera, CardboardMain, OculusMain;
	GameObject visualisationObject;
	GameObject title;

	Island currentIsland;
	ArrayList theIslands;

	GrandCentral grandCentral;

	MLSettings settings;
	//	MLUx ux;

	Material mat;
	//	MLDirector director;

	BackGroundTaskManager backGroundTaskManager;
	IslandVisualisationFilters islandVisualisationFilters;

	// ***************************************************************************************************
	// Unity start & update methods
	// ***************************************************************************************************

	/*
	// Set up an event to be triggered
	public event ChangedEventHandler Changed;

	// Invoke the Changed event;
	protected virtual void OnChanged (EngineChangedEventArgs e)
	{
		// empty eventargs: (EventArgs.Empty);

		if (Changed != null)
			Changed (this, e); // trigger the event
	}


	void testMethod (int i)
	{

		Debug.Log ("Executed testhmethod with value: " + i);
	}


	IEnumerator	testTask (TaskStatus taskStatus)
	{
		for (int i = 0; i < 100; i++) 
		{
			Debug.Log ("Test task executing");
			yield return false;
		}

		taskStatus.isDone = true;
		Debug.Log ("Test task done");
	}


	*/



	void Start ()
	{

		backGroundTaskManager = GameObject.Find ("Root").GetComponent <BackGroundTaskManager> (); 
		settings = GameObject.Find ("Root").GetComponent <MLSettings> (); // get a reference to the settings
		grandCentral = GameObject.Find ("Root").GetComponent <GrandCentral> ();

		grandCentral.GC_Changed += new GC_EventHandler (GC_Event);

//		MainCamera = settings.MainCamera;
//		CardboardMain = settings.CardboardMain;
//		OculusMain = settings.OculusMain; 

		visualisationObject = GameObject.Find ("MainVisualisationObject");

//		backGroundTaskManager.addTask (testTask,testCompletionTask);

		islandVisualisationFilters = new IslandVisualisationFilters (0);




		/*
		RandomTerrain testTerrain = new RandomTerrain (settings.size, 4, settings.initialAmplitude, settings.initialRoughness);
		testTerrain.spawn ();

		DelauneyAlgorithm testDelauney = new DelauneyAlgorithm (settings.initialVerticeCount, settings.size, testTerrain, false, new DelauneyStateHandler(delauneyHandling));

		backGroundTaskManager.addTask (testDelauney.process, testCompletionTask);

*/


		// Get references to the project cameras.
		// Islands have 'camera' gameobjects. In the piece, we'll plot the data from those objects onto the actual project cameras: generic, cardboard, oculus.

	

		// 
		mat = Resources.Load ("Colour01") as Material;
		GameObject.Find ("Cube").GetComponent<Renderer> ().material = mat;

//		goTo ();

		Debug.Log ("setcontroller started");


	}

	public float getHeight (float x, float z)
	{
		float y = 0;

		return y;
	}

	// Update is called once per frame
	void Update ()
	{







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
//		currentCamera = (currentIsland.getCamera (settings.currentCameraNo));

	}

	public void switchToIsland (int index)
	{
		// Switch to a buffered island, drop cam
		if (index < theIslands.Count) {
			currentIsland.setVisible (false);
			currentIsland = (Island)theIslands [index];
			currentIsland.setVisible (true);
//			currentCamera = (currentIsland.getCamera (settings.currentCameraNo));

		}
	}

	Island spawnIsland (int verticeCount, float amplitude, float roughness)
	{
		// Spawn a new island and return a reference
		Island theIsland = new Island (GameObject.Find ("Root"), "Island 01");

		theIsland.setLineColor (settings.lineColor01);

		theIsland.spawnTerrain (settings.size, 4, amplitude, roughness);



		theIsland.spawnDelauney (verticeCount, settings.size);



//		theIsland.spawnFlock ();


	
//		currentCamera = (theIsland.getCamera (settings.currentCameraNo));
			
		return theIsland;
	}



	public void deleteCurrentBuffer ()
	{
		// Delete the current island from the buffer and switch to 0
		Destroy (currentIsland.getGameObject (), 0f);
		theIslands.Remove (currentIsland);
		switchToIsland (0);
//		currentCamera = (currentIsland.getCamera (settings.currentCameraNo));


	}

	// ***************************************************************************************************
	// Story / director methods
	// ***************************************************************************************************

	void GC_Event (object sender, GC_EventArgs e)
	{
//		Debug.Log ("SetController heard GC event");
	
		switch (e.storyEvent) {

		case STORYEVENT.PREPARESET:
//			Debug.Log ("Initialising edit island");
			theIslands = new ArrayList ();
			currentIsland = spawnIsland (settings.initialVerticeCount, settings.initialAmplitude, settings.initialRoughness);
			theIslands.Add (currentIsland);
			Debug.Log ("SetController: set ready");
			grandCentral.notify (STORYEVENT.SETREADY);


			break;

		case STORYEVENT.PREPAREISLANDPLAYBACK:

			Island theIsland = new Island (GameObject.Find ("Root"), "Island");

			GameObject playerObject = new GameObject ("Island Geometry");

			playerObject.transform.parent = theIsland.getGameObject ().transform;

			theIsland.spawnTerrain (settings.size, 4, settings.initialAmplitude, settings.initialRoughness);

			DelauneyAlgorithm testDelauney = new DelauneyAlgorithm (settings.initialVerticeCount, settings.size, theIsland.iTerrain, true, new DelauneyStateHandler (delauneyHandling));

			backGroundTaskManager.addTask (testDelauney.process, delauneyHandlingDone);

			break;

		default:
			break;


		}
	}

	//	GameObject currentCamera;
	GeometryPlayer currentGeometryPlayer;


	string[] chapters = { "none", "islandLoading", "island" };
	int currentChapterIndex = -1;
	string currentChapter;

	void delauneyHandling (Vector3[] vertices, int[] triangles)
	{

		islandVisualisationFilters.parseRawData (ISLANDLOOK.BASIC, GameObject.Find ("Island Geometry"), vertices, triangles);

//		Debug.Log ("Totally handling things here.");
	}

	void delauneyHandlingDone (TaskStatus taskStatus)
	{
//		storyEvent (2);
		Debug.Log ("SetController: island ready for playback");
		grandCentral.notify (STORYEVENT.ISLANDREADYFORPLAYBACK);

//		Debug.Log ("Set");
	}



	public void storyEvent (int i)
	{




		if (i != currentChapterIndex) {

			if (i > chapters.Length)
				i = 0;


//
//			EngineChangedEventArgs e = new EngineChangedEventArgs ();
////			e.thisHappened = "empty";
//			e.newChapter = i;
//
//			OnChanged (e);



			string newChapter = chapters [i];


			switch (newChapter) {

			case "none":
				settings.UICanvas.SetActive (true);
//				showTitle ("Now in", "Editor mode", 2f);
				break;

			case "islandLoading":
				settings.UICanvas.SetActive (false);
			// prepare for island creation animation




			// Spawn a new island and return a reference
				Island theIsland = new Island (GameObject.Find ("Root"), "Island");


				GameObject playerObject = new GameObject ("Island Geometry");

				playerObject.transform.parent = theIsland.getGameObject ().transform;
						

				theIsland.spawnTerrain (settings.size, 4, settings.initialAmplitude, settings.initialRoughness);




				DelauneyAlgorithm testDelauney = new DelauneyAlgorithm (settings.initialVerticeCount, settings.size, theIsland.iTerrain, false, new DelauneyStateHandler (delauneyHandling));

				backGroundTaskManager.addTask (testDelauney.process, delauneyHandlingDone);



//				theIsland.spawnDelauney (settings.initialVerticeCount, settings.size, currentGeometryPlayer); // This is where the blocking occurs. 



				/*

				theIsland.initialiseCameras ();
				theIsland.addOrbitCamera ();
				settings.currentCameraNo = 0;


				Destroy (currentIsland.getGameObject (), 0f);
				currentIsland.Detach ();

				theIslands.Remove (currentIsland);



				currentCamera = (theIsland.getCamera (settings.currentCameraNo));

				currentIsland = theIsland;

				theIslands.Add (currentIsland);
//				showTitle ("Part 1","Delauney", 5f);
*/

				break;

			case "island":


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
		storyEvent (settings.chapter);
//		currentCamera = (currentIsland.getCamera (settings.currentCameraNo));

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





	// ***************************************************************************************************
	// Public access methods
	// ***************************************************************************************************
	//	public GameObject getCurrentCamera ()
	//	{
	//		return		currentCamera;
	//	}

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
