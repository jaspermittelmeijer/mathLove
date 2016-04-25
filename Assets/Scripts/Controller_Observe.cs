using UnityEngine;
using System.Collections;

public class Controller_Observe : MonoBehaviour
{
	GameObject MainCamera, CardboardMain, OculusMain;
	GameObject self;
	//	GameObject visualisation
	GrandCentral grandCentral;
	MLSettings settings;
	BackGroundTaskManager backGroundTaskManager;
	Controller_Set setController;
	GameObject currentCamera;
	ArrayList cameras;

	GameObject title;

	// Use this for initialization
	void Start ()
	{
		backGroundTaskManager = GameObject.Find ("Root").GetComponent <BackGroundTaskManager> (); 
		settings = GameObject.Find ("Root").GetComponent <MLSettings> (); // get a reference to the settings
		grandCentral = GameObject.Find ("Root").GetComponent <GrandCentral> ();
		setController = GameObject.Find ("Root").GetComponent <Controller_Set> ();

		title = GameObject.Find ("Title");

		self = GameObject.Find ("Observe");

//		visualisationObject = GameObject.Find ("MainVisualisationObject");


		grandCentral.GC_Changed += new GC_EventHandler (GC_Event);

		MainCamera = settings.MainCamera;
		CardboardMain = settings.CardboardMain;
		OculusMain = settings.OculusMain; 

		initialiseCameras ();
		Debug.Log ("ObserveController started");
	}
	
	// Update is called once per frame
	void Update ()
	{

		// Call counter script for onscreen titles
		if (titleTimer ()) {
			title.SetActive (false);
		}
		
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
				//				title.transform.position = currentCamera.transform.position;
				//				title.transform.rotation = currentCamera.transform.rotation;

				setTitleTransformation (currentCamera.transform.position, currentCamera.transform.rotation);


			}
			break;

		default:
			MainCamera.transform.position = currentCamera.transform.position;
			MainCamera.transform.rotation = currentCamera.transform.rotation;
			//			title.transform.position = currentCamera.transform.position;
			//			title.transform.rotation = currentCamera.transform.rotation;
			setTitleTransformation (currentCamera.transform.position, currentCamera.transform.rotation);
			break;


		}
	}

	void GC_Event (object sender, GC_EventArgs e)
	{

		switch (e.storyEvent) {

		case STORYEVENT.PREPARECAMERAS:
	
			currentCamera = newOrbitCamera ("Orbitcam");
			cameras.Add (currentCamera);
			Debug.Log ("ObserveController: cameras ready");
			grandCentral.notify (STORYEVENT.CAMERASREADY);

			break;

		case STORYEVENT.PREPAREISLANDPLAYBACK:
			
			settings.UICanvas.SetActive (false);
			break;

		case STORYEVENT.BEGINISLANDPLAYBACK:

			showTitle ("Part 1", "Delauney Algorithm", 3);


			break;

		default:
			break;


		}
	}

	void initialiseCameras ()
	{
		cameras = new ArrayList ();
		cameras.Add (newDefaultCamera ("Default camera"));
		currentCamera = (GameObject)cameras [0];

	}

	GameObject newDefaultCamera (string _name)
	{
		GameObject workingObject = new GameObject (_name);
		workingObject.transform.parent = self.transform;

		return workingObject;
	}


	public void addOrbitCamera ()
	{
		cameras.Add (newOrbitCamera ("Orbitcam"));

	}

	//	public void addDefaultCameras ()
	//	{
	//
	//		cameras.Add (newOrbitCamera ("Orbitcam"));
	////		cameras.Add (addTargetCamera ("TargetCam01",  GameObject GOES HERE));
	////		cameras.Add (addStaticCamera ("StaticCam02"));
	////		cameras.Add (addStaticCamera ("StaticCam03"));
	//	}

	public GameObject getCurrentCamera ()
	{
		return		currentCamera;
	}


	GameObject newOrbitCamera (string _name)
	{
		GameObject workingObject = new GameObject (_name + "_anchor");
		workingObject.transform.parent = self.transform;
		workingObject.transform.position = new Vector3 (0.5f * settings.size, 0.0f * settings.size, 0.5f * settings.size);

		GameObject newCamera = new GameObject (_name);
		newCamera.transform.parent = workingObject.transform;

		Vector3 cameraPosition = new Vector3 (1.5f * settings.size, 0.1f * settings.size, 1.5f * settings.size);

		newCamera.transform.position = cameraPosition;
		newCamera.transform.localRotation = Quaternion.LookRotation (workingObject.transform.position - newCamera.transform.position, Vector3.up);

		workingObject.AddComponent <OrbitCam> ();


		return newCamera;
	}

	GameObject newStaticCamera (string _name)
	{
		// drop a new camera at a random point
		GameObject newCamera = new GameObject (_name);
		newCamera.transform.parent = self.transform;

		Vector3 cameraPosition = new Vector3 (UnityEngine.Random.Range (0.0f, settings.size), 0.0f, UnityEngine.Random.Range (0.0f, settings.size));

		//		cameraPosition.y = getHeight (cameraPosition.x, cameraPosition.z) + 0.5f * settings.initialAmplitude;
		cameraPosition.y = setController.getHeight (cameraPosition.x, cameraPosition.z) + 0.1f * settings.initialAmplitude;

		newCamera.transform.position = cameraPosition;
		newCamera.transform.localRotation = Quaternion.LookRotation (new Vector3 (settings.size * .25f, 0f, settings.size * .25f) - newCamera.transform.position, Vector3.up);

		return newCamera;
	}

	GameObject newTargetCamera (string _name, GameObject _target)
	{
		GameObject newCamera = new GameObject (_name);
		newCamera.transform.parent = self.transform;

//		GameObject theTarget = iFlock.GetComponent<Flock> ().getBoid (_targetBoid);

		FollowCam cameraControl = newCamera.AddComponent <FollowCam> ();

		cameraControl.setTarget (_target);

		cameraControl.setTargetDebug (VisualisationObject.newNull (_target.transform.position, 0.25f, self));

		return newCamera;
	}

	void setTitleTransformation (Vector3 pos, Quaternion rot)
	{
		title.transform.position = pos;
		title.transform.rotation = rot;



	}

	void showTitle (string sub, string main, float theTime)
	{
		title.SetActive (true);
		title.transform.FindChild ("Main").GetComponent<TextMesh> ().text = main;
		title.transform.FindChild ("Sub").GetComponent<TextMesh> ().text = sub;
		c = theTime;

	}

	private float c = 1;

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



}
