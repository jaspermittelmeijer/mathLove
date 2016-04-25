using UnityEngine;
using System.Collections;
using System;




// Declare a delegate for Grand Central event handling, so controllers can subscribe
public delegate void GC_EventHandler (object sender, GC_EventArgs e);

// Extend eventargs to hold our event info.
public class GC_EventArgs : EventArgs
{
	public STORYEVENT storyEvent;


	public GC_EventArgs (STORYEVENT _storyEvent) : base () // extend the contstructor 
	{ 
		storyEvent = _storyEvent;
		

		// Fill the args with empty values to prevent having to check for null values
	

	}
}





public class GrandCentral : MonoBehaviour
{
	bool started = false;
	CHAPTER currentChapter;


	// Set up an event to be triggered
	public event GC_EventHandler GC_Changed;

	// Invoke the event;
	protected virtual void OnChanged (GC_EventArgs e)
	{
		// empty eventargs: (EventArgs.Empty);




		if (GC_Changed != null)
			GC_Changed (this, e); // trigger the event
	}

	// Use this for initialization
	void Start ()
	{

		currentChapter = CHAPTER.VOID;

		Debug.Log ("Grand Central started");

	}
	
	// Update is called once per frame
	void Update ()
	{
		if (!started) {
			// Start the chain of events
			notify (STORYEVENT.BEGIN);
			started = true;
		}
	
	}



	public void notify (STORYEVENT storyEvent)
	{

		GC_EventArgs e;


		switch (storyEvent) {

	

		case STORYEVENT.BEGIN:
			Debug.Log ("GC: prepare set");
			 e = new GC_EventArgs (STORYEVENT.PREPARESET);
			OnChanged (e);
			break;


		case STORYEVENT.SETREADY:
			Debug.Log ("GC: prepare cameras");
			 e = new GC_EventArgs (STORYEVENT.PREPARECAMERAS);
			OnChanged (e);
			break;

		case STORYEVENT.CAMERASREADY:
			Debug.Log ("GC: release flock");
			e = new GC_EventArgs (STORYEVENT.RELEASEFLOCK);
			OnChanged (e);
			break;


		case STORYEVENT.NEXTCHAPTER:
			Debug.Log ("GC: skip to next chapter / prepare island playback");
			e = new GC_EventArgs (STORYEVENT.PREPAREISLANDPLAYBACK);
			OnChanged (e);
			break;

		case STORYEVENT.PREVIOUSCHAPTER:
			Debug.Log ("GC: skip to previous chapter");
		
			break;

		case STORYEVENT.ISLANDREADYFORPLAYBACK:
			Debug.Log ("GC: begin island playback");
			e = new GC_EventArgs (STORYEVENT.BEGINISLANDPLAYBACK);
			OnChanged (e);
			break;


		case STORYEVENT.SWITCHTOEDITMODE:
			Debug.Log ("GC: switch to edit mode");
			e = new GC_EventArgs (STORYEVENT.SWITCHTOEDITMODE);
			OnChanged (e);
			break;


		default:
			break;
		}

	}





}
