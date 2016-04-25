using UnityEngine;
using System.Collections;
using UnityEngine.UI;



public class UserInterface : MonoBehaviour
{

	// Class to handle any interaction with the user.


	bool looking;
	GameObject theCamera;
	float rotationX = 0F;
	float rotationY = 0F;
	float sensitivityX = 15F;
	float sensitivityY = 15F;
	float smoothMouseX, smoothMouseY, mouseRawX, mouseRawY;
	Quaternion originalRotation;


	int verticeSliderValue;
	float amplitudeSliderValue, roughnessSliderValue;

	public bool visualDebug;
	public Text UI_DebugText, UI_VerticeSliderText, UI_AmplitudeSliderText, UI_RoughnessSliderText;
	public GameObject UI_VerticeSlider, UI_AmplitudeSlider, UI_RoughnessSlider;


	public Button[] bufferButtons;

	Controller_Set engine;
	MLSettings settings;
	Controller_Deus deusController;

	void Start ()
	{
		engine = GameObject.Find ("Root").GetComponent <Controller_Set> ();
		settings = GameObject.Find ("Root").GetComponent <MLSettings> ();

		deusController = GameObject.Find ("Deus").GetComponent <Controller_Deus> ();



		theCamera = settings.MainCamera; //

		UI_VerticeSlider.GetComponent <Slider> ().value = settings.initialVerticeCount;
		verticeSliderChanged (settings.initialVerticeCount); 

		UI_AmplitudeSlider.GetComponent <Slider> ().value = settings.initialAmplitude;
		amplitudeSliderChanged (settings.initialAmplitude); 

		UI_RoughnessSlider.GetComponent <Slider> ().value = settings.initialRoughness;
		roughnessSliderChanged (settings.initialRoughness); 

		for (int i = 1; i < 5; i++) {
			bufferButtons [i].gameObject.SetActive (false);
		}



	}
//
//	 void setCamera (GameObject _camera)
//	{
//		theCamera = _camera;
//
//	}


	
	// Update is called once per frame
	void Update ()
	{



		// Check for keyboard input

		if (Input.GetKeyDown ("c")) {
			Debug.Log ("Switch Camera");
			if (settings.thePlatform == "OSX") {
				OVRManager.display.RecenterPose ();
				//				OVRDisplay.RecenterPose	();
			}

//			settings.currentCameraNo = (settings.currentCameraNo + 1) % engine.getCurrentIsland ().getCameraCount ();




			engine.goTo ();
//			setCamera (engine.getCurrentCamera ());

		}

		if (Input.GetKeyDown ("f")) {
			while (engine.getCurrentIsland ().getDelauney ().flipAllTriangles ())
				;

			Debug.Log ("Flipping triangles");
		}
		if (Input.GetKeyDown (KeyCode.Return)) {
			engine.reSpawnCurrentIsland (settings.initialVerticeCount, settings.initialAmplitude, settings.initialRoughness);
			Debug.Log ("Enter");
		}

		if (Input.GetKeyDown ("n")) {
//			engine.nextChapter ();
			deusController.notify (DEUSEVENT.NEXTCHAPTER);


		}
		if (Input.GetKeyDown ("p")) {
//			engine.previousChapter ();
			deusController.notify (DEUSEVENT.PREVIOUSCHAPTER);


		}

		if (Input.GetKeyDown ("g")) {
//			engine.goTo ();

		}

		if (Input.GetKeyDown ("x")) {
//			engine.exit ();SWITCHTOEDITMODE
			deusController.notify (DEUSEVENT.EXIT);
		}

	

	

	

//		if (settings.thePlatform == "OSX") {
//			if (Input.GetKeyDown (KeyCode.Return)) {
//				world.reSpawnCurrentIsland (settings.initialVerticeCount, settings.initialAmplitude, settings.initialRoughness);
//			}
//		}


		// Check for cardboard input

	
		if (settings.thePlatform == "iOS") {
			if (Cardboard.SDK.CardboardTriggered) {
				Debug.Log ("trigger");
				engine.reSpawnCurrentIsland (settings.initialVerticeCount, settings.initialAmplitude, settings.initialRoughness);


			}
		}

	




		// Call counter script for onscreen debug messages
		if (debugCounter ()) {
			debugMessage ("", -1.0f);
		}




		GameObject.Find ("Cube").transform.rotation = Input.gyro.attitude;




		if (settings.thePlatform != "iOS") {

//			setCamera (engine.getCurrentCamera ());

			if (Input.GetMouseButtonDown (0)) {
				//			Debug.Log("Start looking");

				looking = true;
				originalRotation = theCamera.transform.localRotation;
				rotationX = 0F;
				rotationY = 0F;
			}

			if (Input.GetMouseButtonUp (0)) {
				//			Debug.Log("Stop looking");

				looking = false;


			}



			if (looking) {


				mouseRawX = Input.GetAxisRaw ("Mouse X");
				mouseRawY = Input.GetAxisRaw ("Mouse Y");

				smoothMouseX = Mathf.Lerp (smoothMouseX, mouseRawX, 1f / 3f);
				smoothMouseY = Mathf.Lerp (smoothMouseY, mouseRawY, 1f / 3f);

				rotationX += smoothMouseX * sensitivityX;
				rotationY += smoothMouseY * sensitivityY;

				Quaternion xQuaternion = Quaternion.AngleAxis (rotationX, Vector3.up);
				Quaternion yQuaternion = Quaternion.AngleAxis (rotationY, -Vector3.right);


				theCamera.transform.localRotation = originalRotation * xQuaternion * yQuaternion;
//				theCamera.transform.localRotation = Quaternion.Euler ( theCamera.transform.eulerAngles.x,  theCamera.transform.eulerAngles.y, 0f);

				theCamera.transform.eulerAngles = new Vector3 (theCamera.transform.eulerAngles.x, theCamera.transform.eulerAngles.y, 0f);



//				originalRotation = theCamera.transform.rotation;
			}
		}



	}

	public void bufferButtonClicked (int buffer)
	{
		debugMessage ("Switch to buffer: " + buffer, 3f);
		engine.switchToIsland (buffer);
//		setCamera (engine.getCurrentCamera ());
	}


	public void verticeSliderChanged (float passValue)
	{

		UI_VerticeSliderText.text = "" + passValue;
		verticeSliderValue = Mathf.FloorToInt (passValue);

	}

	public void amplitudeSliderChanged (float passValue)
	{

		UI_AmplitudeSliderText.text = "" + passValue;
		amplitudeSliderValue = passValue;

	}

	public void roughnessSliderChanged (float passValue)
	{

		UI_RoughnessSliderText.text = "" + passValue;
		roughnessSliderValue = passValue;

	}


	public void deleteIslandButtonClicked ()
	{

		if (engine.getNumberOfIslands () > 1) {
			bufferButtons [engine.getCurrentBufferSize () - 1].gameObject.SetActive (false);
			engine.deleteCurrentBuffer ();
			debugMessage ("Deleted island", 5f);

//			setCamera (engine.getCurrentCamera ());
//			setCamera (MLDirector?);
		}

	}

	public void spawnButtonClicked ()
	{


		if (engine.getNumberOfIslands () < 5) {
			engine.addNewIsland (verticeSliderValue, amplitudeSliderValue, roughnessSliderValue);
			debugMessage ("Created new island", 5f);
			bufferButtons [engine.getNumberOfIslands () - 1].gameObject.SetActive (true);
//			setCamera (engine.getCurrentCamera());
		}


	}


	private float c;

	private bool debugCounter ()
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

	public void debugMessage (string theMessage, float theTime)
	{
		Debug.Log (theMessage);

		if (visualDebug) {
			UI_DebugText.text = theMessage;
			c = theTime;
		}
	}
}
