using UnityEngine;
using System.Collections;

public class Controller_Agents : MonoBehaviour
{
	GameObject MainCamera, CardboardMain, OculusMain;
	GrandCentral grandCentral;
	MLSettings settings;
	BackGroundTaskManager backGroundTaskManager;
	GameObject flock;
	GameObject self;

	// Use this for initialization
	void Start ()
	{
		backGroundTaskManager = GameObject.Find ("Root").GetComponent <BackGroundTaskManager> (); 
		settings = GameObject.Find ("Root").GetComponent <MLSettings> (); // get a reference to the settings
		grandCentral = GameObject.Find ("Root").GetComponent <GrandCentral> ();

		grandCentral.GC_Changed += new GC_EventHandler (GC_Event);

		self = GameObject.Find ("Agents");

		MainCamera = settings.MainCamera;
		CardboardMain = settings.CardboardMain;
		OculusMain = settings.OculusMain; 
		Debug.Log ("AgentController started");
	}

	// Update is called once per frame
	void Update ()
	{

	}

	void GC_Event (object sender, GC_EventArgs e)
	{
		switch (e.storyEvent) {

		case STORYEVENT.RELEASEFLOCK:
			
			flock = new GameObject ("Flock");
			flock.transform.parent = self.transform;
			flock.AddComponent <Flock> ().createFlock ();

			break;

		default:
			break;


		}
	}
}
