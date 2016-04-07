using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
	// Class to hold settings. No active code should reside here.

	public GameObject MainCamera, CardboardMain, OculusMain, UICanvas;
	public bool simulate;
	public string simulationPlatform;
	public string thePlatform;
	bool onScreenMessages;
	public int initialVerticeCount = 90;
	public float initialAmplitude = 3f;
	public float initialRoughness = 0.7f;
	public float size;
	public int flockSize;
	public float initialSpeed = 7f;
	public float initialWanderAmplitude = 50f;
	public float initialWanderRate = 0.5f;
//	public bool flightCamOn;
	public int currentCameraNo = 0;
	public bool directorOn =true;

	Interaction interaction;

	public Color lineColor01;

	void Start ()
	{

		interaction = GameObject.Find ("Root").GetComponent <Interaction> ();

		// Call script to apply settings that depend on platform (eg touch vs mouse)
		applyPlatformSettings ();

	}
	
	// Update is called once per frame
	void Update ()
	{


	}

	void applyPlatformSettings ()
	{


		switch (Application.platform) {

		case RuntimePlatform.OSXEditor:
			thePlatform = "Unity";
			break;

		case RuntimePlatform.OSXPlayer:
			thePlatform = "OSX";
			break;

		case RuntimePlatform.WebGLPlayer:
			thePlatform = "WebGL";
			break;

		case RuntimePlatform.IPhonePlayer:
			thePlatform = "iOS";
			break;

		}

		if (simulate)
			thePlatform = simulationPlatform;
		
		switch (thePlatform) {
			
		case "Unity":
			onScreenMessages = true;
			UICanvas.SetActive (true);
			MainCamera.SetActive (true);
			OculusMain.SetActive (false);

			CardboardMain.SetActive (false);
			break;
			
		case "OSX":
			onScreenMessages = true;
			UICanvas.SetActive (false);
			MainCamera.SetActive (false);
			OculusMain.SetActive (true);

			CardboardMain.SetActive (false);
			break;

		case "WebGL":
			onScreenMessages = false;
			UICanvas.SetActive (false);
			MainCamera.SetActive (true);
			OculusMain.SetActive (false);

			CardboardMain.SetActive (false);
			break;


		case "iOS":
			onScreenMessages = false;
			UICanvas.SetActive (false);
			MainCamera.SetActive (false);
			OculusMain.SetActive (false);

			CardboardMain.SetActive (true);
//			Debug.Log ("xxxxxxxxxxxxxxxxxx");
			break;
			
		}


	}


}
