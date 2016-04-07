using UnityEngine;
using System.Collections;



public class FollowCam : MonoBehaviour
{

	// Class to control automatic moving cameras


	Settings settings;
	public GameObject target, targetDebug;
	Vector3 targetPositionSmoothed;
	VisualLineDebug lineDebug;
	float distance;
	Quaternion steerRotation;
	float keepDistance = 5f;
	Vector3 steerPosition;
	Vector3 xAxis, yAxis, zAxis;
	bool following;
	float collisionRange = 5f;
	World world;
	bool hasLock = false;
	GameObject cSelf;
 
	void Start ()
	{
	}





	public void setTarget (GameObject _target)
	{
		settings = GameObject.Find ("Root").GetComponent <Settings> ();
		world = GameObject.Find ("Root").GetComponent <World> ();
		cSelf = transform.gameObject;

		lineDebug = GameObject.Find ("MainVisualisationObject").GetComponent<VisualLineDebug> ();

		target = _target;
		targetPositionSmoothed = target.transform.position;
		cSelf.transform.rotation = Quaternion.LookRotation (target.transform.position - cSelf.transform.position, Vector3.up);
		hasLock = false;

	}

	void followTarget ()
	{
		targetPositionSmoothed += 0.05f * (target.transform.position - targetPositionSmoothed);

		Vector3 deltaToTarget = targetPositionSmoothed - cSelf.transform.position;
		Debug.DrawLine (target.transform.position, targetPositionSmoothed, Color.white);
		lineDebug.addLine (target.transform.position, targetPositionSmoothed);

		distance = deltaToTarget.magnitude;
		targetDebug.transform.position = targetPositionSmoothed;

		if (following) {

			steerRotation = Quaternion.LookRotation (targetPositionSmoothed - cSelf.transform.position, Vector3.up);
//			if (distance > keepDistance) {
			steerPosition = cSelf.transform.position + 0.001f * (distance - keepDistance) * deltaToTarget;
//			}
		}



	}

	void avoidTerrain ()
	{
		RaycastHit theHit;
		if (Physics.Raycast (transform.position, zAxis, out theHit, collisionRange)) {
			// need to pull up
			//			Debug.Log("I need to pull up: " + theHit.distance);
			//			seekTarget = false;
			xAxis = transform.rotation * Vector3.right;
			float proximity = (collisionRange - theHit.distance) / collisionRange;

			steerRotation = Quaternion.AngleAxis (proximity * proximity * -90f, xAxis) * steerRotation;


			//			direction = transform.rotation * Vector3.forward;
			if (theHit.distance < 5f) {
				Debug.Log ("Avoiding collision");
//				following = false;
			}

			//			Debug.DrawRay (transform.position, zAxis * theHit.distance, Color.red);
//			lineDebug.addLine (transform.position, transform.position + zAxis * theHit.distance);



		} else {
			following = true;
		}


	}

	void steer ()
	{
		



//		if (distance > keepDistance) {
//			steerPosition 
//		}

		cSelf.transform.position = steerPosition;

		cSelf.transform.rotation = steerRotation;




	}

	public void setTargetDebug (GameObject _targetDebug)
	{
		targetDebug = _targetDebug;

	}

	void setAxis ()
	{
		zAxis = transform.rotation * Vector3.forward;
		xAxis = transform.rotation * Vector3.right;
		yAxis = transform.rotation * Vector3.up;
	}



	// Update is called once per frame
	void Update ()
	{
//		theCamera.transform.position = target.transform.position;
//		theCamera.transform.rotation = target.transform.rotation;

//		flightCamOn = settings.flightCamOn;


			followTarget ();
			avoidTerrain ();
			steer ();
			setAxis ();




	}
}



public class OrbitCam : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

		//		Quaternion delta = Quaternion.AngleAxis(10f * Time.deltaTime, new Vector3 (0f,0.75f,0.25f));
		Quaternion delta = Quaternion.Euler (-1f*Time.deltaTime,10f*Time.deltaTime,0f);

		transform.rotation = transform.rotation * delta;

		transform.GetChild(0).eulerAngles = new Vector3(transform.GetChild(0).eulerAngles.x,transform.GetChild(0).eulerAngles.y,0f);
		//		transform.eulerAngles = new Vector3(transform.eulerAngles.x,transform.eulerAngles.y,0f);

		//		Quaternion rotation = Quaternion.Euler(0, 30, 0);

	}
}
