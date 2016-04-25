using UnityEngine;
using System.Collections;

public enum DEUSEVENT
{
	NEXTCHAPTER,
	PREVIOUSCHAPTER,
	EXIT
}
;


public class Controller_Deus : MonoBehaviour
{
	
	GameObject MainCamera, CardboardMain, OculusMain;
	GrandCentral grandCentral;
	MLSettings settings;
	BackGroundTaskManager backGroundTaskManager;

	// Use this for initialization
	void Start ()
	{
		backGroundTaskManager = GameObject.Find ("Root").GetComponent <BackGroundTaskManager> (); 
		settings = GameObject.Find ("Root").GetComponent <MLSettings> (); // get a reference to the settings
		grandCentral = GameObject.Find ("Root").GetComponent <GrandCentral> ();

		grandCentral.GC_Changed += new GC_EventHandler (GC_Event);

		MainCamera = settings.MainCamera;
		CardboardMain = settings.CardboardMain;
		OculusMain = settings.OculusMain; 

		Debug.Log ("DeusController started");
	}

	// Update is called once per frame
	void Update ()
	{

	}

	public void notify (DEUSEVENT deusEvent)
	{

		switch (deusEvent) {

		case DEUSEVENT.NEXTCHAPTER:
			grandCentral.notify (STORYEVENT.NEXTCHAPTER);
			break;
		
		case DEUSEVENT.PREVIOUSCHAPTER:
			grandCentral.notify (STORYEVENT.PREVIOUSCHAPTER);
			break;
	
		case DEUSEVENT.EXIT:
			grandCentral.notify (STORYEVENT.SWITCHTOEDITMODE);
			break;

		default:
			break;
		}

	}


	void GC_Event (object sender, GC_EventArgs e)
	{
		switch (e.storyEvent) {

		case STORYEVENT.VOID:

			break;

		default:
			break;


		}
	}
}
