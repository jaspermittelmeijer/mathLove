using UnityEngine;
using System.Collections;

public class Flock : MonoBehaviour
{
	// Class to hold a flock. Note that this doesn't have to be a monobehaviour the way it is now.

	GameObject flockObject;
	Settings settings;
	GameObject[] boids;


	// Use this for initialization
	void Start ()
	{
		

	}

	public void createFlock (){
		settings = GameObject.Find ("Root").GetComponent <Settings> (); // get a reference to the settings
		flockObject = transform.gameObject;
		boids = new GameObject[settings.flockSize];

		// Create the flock
		for (int i = 0; i < settings.flockSize; i++) {
			boids [i] = VisualisationObject.newBasicBoid (new Vector3 ((0.25f+Random.Range(-1,1)*.2f)*settings.size,Random.Range(3f,8f),(0.25f+Random.Range(-1,1)*.2f)*settings.size), 0.5f, flockObject);

			boids [i].AddComponent<Boid> ().setValues(boids,settings.initialSpeed,settings.initialWanderAmplitude,settings.initialWanderRate);

			boids [i].GetComponent<Boid>().childNull = VisualisationObject.newBasicBoid ( boids[i].transform.position,0.25f, boids [i]);
			//			boids [i].GetComponent<Boid> ().passFlock(boids);


		}



	}

	public GameObject getBoid (int i){

		return (boids [i]);
	}

	// Update is called once per frame
	void Update ()
	{
	
	}
}


public class Boid : MonoBehaviour
{
	// Class to describe a 'boid', part of a flock.
	// Note that this should be agnostic, in the sense that the way the boids are visualised must be independent of its mechanical workings.


	public GameObject childNull;

	public Vector3 zAxis;

	Vector3 yAxis, xAxis, wander;
	Vector3 centerWeighted;
	float speed, wanderRate, wanderAmplitude, centerAttraction;
	float collisionRange, influenceRange, influenceRangeS;
	Quaternion start, end, smooth;
	bool wandering;

	GameObject[] theFlock;
	GameObject iSelf;
	VisualLineDebug lineDebug;


	// Use this for initialization
	void Start ()
	{

	}

	public void setValues (GameObject[] _flock, float _speed, float _wanderAmplitude, float _wanderRate)
	{
		iSelf = transform.gameObject;
		transform.rotation = Quaternion.LookRotation (new Vector3 (0.1f, -0.9f, 0.1f), Vector3.up);

		setAxis ();

		wander = zAxis;

		theFlock = _flock;

		speed = _speed;
		wanderAmplitude = _wanderAmplitude;
		wanderRate = _wanderRate;

		collisionRange = 5f;
		influenceRange = 5f;
		influenceRangeS = 3f;

		centerAttraction = 0.01f;
		centerWeighted = new Vector3 (centerAttraction * 25f, 0f, centerAttraction * 25f);

		lineDebug = GameObject.Find ("MainVisualisationObject").GetComponent<VisualLineDebug> ();

	}

	void setAxis ()
	{
		zAxis = transform.rotation * Vector3.forward;
		xAxis = transform.rotation * Vector3.right;
		yAxis = transform.rotation * Vector3.up;
	}



	//	public void passFlock (GameObject[] _flock){
	//		theFlock = _flock;
	//
	//	}

	void steer2 ()
	{
		float force = Vector3.Angle (zAxis, wander);



		childNull.transform.rotation = Quaternion.LookRotation (wander, Vector3.up);
		//		Quaternion 



		Quaternion newRotation = Quaternion.Slerp (transform.rotation, childNull.transform.rotation, force / 720f);


		//		Quaternion delta = Quaternion.Inverse (transform.rotation) * newRotation;

		//		zAxis = transform.rotation * Vector3.forward;

		//		float turn = 0f;
		//		Vector3 axis = yAxis;

		Vector3 pov = Quaternion.Inverse (transform.rotation) * wander;


		//		float turn = delta.eulerAngles.y;
		Quaternion roll = Quaternion.AngleAxis (-0.5f * pov.x, zAxis);

		transform.rotation = newRotation * roll;

		//		transform.rotation *= roll;




		//		transform.rotation = Quaternion.Lerp (transform.rotation, childNull.transform.rotation, force / 720);

		zAxis = transform.rotation * Vector3.forward;
		//		yAxis = transform.rotation * Vector3.up;

	}

	void steer ()
	{

		/*
		float force = Vector3.Angle  (zAxis, wander);



		childNull.transform.rotation = Quaternion.LookRotation(wander,Vector3.up);

		transform.rotation = Quaternion.Lerp (transform.rotation, childNull.transform.rotation, force / 720);

		zAxis = transform.rotation * Vector3.forward;

*/

		//		float force = Vector3.Angle  (zAxis, wander)/90;
		float force = -5f;

		childNull.transform.rotation = Quaternion.LookRotation (wander, Vector3.up);

		// our boid can steer over x and over z. but not over y. (more bird-like)
		// if we take our 'wander' vector, and correct it for our current rotation we have a POV

		Vector3 pov = Quaternion.Inverse (transform.rotation) * wander;

		// from here we can steer up to the y component and we rotate right to the x component, in a euler kind of way
		//		float up = Mathf.Atan2 (pov.y, pov.z);
		//
		//		if (up > 0f) {
		//
		//
		//		}
		//
		float angleX = Mathf.Atan2 (pov.y, pov.z) * Mathf.Rad2Deg;
		float angleY = Mathf.Atan2 (pov.x, pov.z) * Mathf.Rad2Deg;


		float roll = 0;
		float up = 0;

		//		Debug.Log ("x: " + angleX+ " y: " + angleY);
		/*
		if (pov.y >= 0) {
			

			//we'll steer upp
			if (pov.x >= 0) {
				//we'll steer up, and to the right
				Debug.Log ("Steer right: "+ angleY + " up: " + Mathf.Atan2 (pov.y,Mathf.Abs(pov.z))* Mathf.Rad2Deg);

					


			} else {
				//we'll steer up, and to the left

//				Debug.Log ("Steer left, up");
				Debug.Log ("Steer left: "+ angleY + " up: " + Mathf.Atan2 (pov.y,Mathf.Abs(pov.z))*  Mathf.Rad2Deg);
			}


		} else {
			//we'll steer down

			if (pov.x >= 0) {
		
				Debug.Log ("Steer right: "+ angleY + " down: " + Mathf.Atan2 (pov.y,Mathf.Abs(pov.z)) * Mathf.Rad2Deg);


			} else {
				Debug.Log ("Steer left: "+ angleY + " down: " + Mathf.Atan2 (pov.y,Mathf.Abs(pov.z)) * Mathf.Rad2Deg);

			}
		}
*/
		up = -1f * Mathf.Atan2 (pov.y, Mathf.Abs (pov.z)) * Mathf.Rad2Deg * 0.1f;
		roll = angleY * 0.1f;



		//		float up = Mathf.Atan2 (pov.y, pov.z);


		//		float roll = Mathf.Atan2 (pov.x, pov.y);
		//		float roll=0;

		//		Quaternion steer = Quaternion.Euler (-0.1f * up * Mathf.Rad2Deg, 0f, -0.1f* roll);
		Quaternion steer = Quaternion.Euler (up, roll, -0.5f * roll);
		//		Quaternion steer = Quaternion.Euler (0,0,0);




		transform.rotation *= steer;


		zAxis = transform.rotation * Vector3.forward;
		/*
		setAxis ();

		Debug.DrawRay (new Vector3 (25f,25f,25f), wander, Color.cyan);

//		Debug.DrawRay (new Vector3 (25f,25f,25f), transform.rotation * new Vector3 (pov.x,0f,pov.z), Color.black);
		Debug.DrawRay (new Vector3 (25f,25f,25f), pov.x * xAxis, Color.black);
		Debug.DrawRay (new Vector3 (25f,25f,25f), pov.y * yAxis, Color.black);
		Debug.DrawRay (new Vector3 (25f,25f,25f), pov.z * zAxis, Color.black);

//		Debug.DrawRay (new Vector3 (25f,25f,25f), transform.rotation * pov, Color.cyan);


//		Debug.DrawRay (new Vector3 (25f,25f,25f), wander, Color.cyan);


		Debug.DrawRay (new Vector3 (25f,25f,25f), zAxis, Color.blue);

		Debug.DrawRay (new Vector3 (25f,25f,25f), xAxis, Color.red);
		Debug.DrawRay (new Vector3 (25f,25f,25f), yAxis, Color.green);

*/





	}


	void avoidTerrain ()
	{
		//		Debug.DrawRay (transform.position, zAxis * collisionRange, Color.cyan);

		RaycastHit theHit;
		if (Physics.Raycast (transform.position, zAxis, out theHit, collisionRange)) {
			// need to pull up
			//			Debug.Log("I need to pull up: " + theHit.distance);
			//			seekTarget = false;
			xAxis = transform.rotation * Vector3.right;
			float proximity = (collisionRange - theHit.distance) / collisionRange;

			wander = Quaternion.AngleAxis (proximity * proximity * -90f, xAxis) * wander;


			//			direction = transform.rotation * Vector3.forward;
			if (theHit.distance < 4f) {
				//				Debug.Log ("Float");
				wandering = false;
			}

			//			Debug.DrawRay (transform.position, zAxis * theHit.distance, Color.red);
			lineDebug.addLine (transform.position, transform.position + zAxis * theHit.distance);



		} else {
			wandering = true;
		}

	}


	void align ()
	{

		Vector3 v = Vector3.zero;
		Vector3 c = Vector3.zero;
		Vector3 s = Vector3.zero;

		float cv = 0;
		float cc = 0;
		float cs = 0;


		foreach (GameObject boid in theFlock) {
			if (boid != iSelf) {
				float distance = Vector3.Distance (transform.position, boid.transform.position);
				Boid theBoid = boid.GetComponent<Boid> ();

				if (distance < influenceRange) {

					v.x += theBoid.zAxis.x * (influenceRange - distance) / influenceRange;
					v.y += theBoid.zAxis.y * (influenceRange - distance) / influenceRange;
					v.z += theBoid.zAxis.z * (influenceRange - distance) / influenceRange;

					cv += (influenceRange - distance) / influenceRange;
				}
				if (distance < influenceRange) {
					//					Boid theBoid = boid.GetComponent<Boid> ();


					c.x += boid.transform.position.x * (influenceRange - distance) / influenceRange;
					;
					c.y += boid.transform.position.y * (influenceRange - distance) / influenceRange;
					;
					c.z += boid.transform.position.z * (influenceRange - distance) / influenceRange;
					;


					cc += (influenceRange - distance) / influenceRange;
				}

				c += centerWeighted;
				cc += centerAttraction;


				//				if (Vector3.Magnitude(boid.transform.position) > 


				if (distance < influenceRangeS) {
					//					Boid theBoid = boid.GetComponent<Boid> ();

					s.x += boid.transform.position.x * (influenceRangeS - distance) / influenceRangeS;
					;
					s.y += boid.transform.position.y * (influenceRangeS - distance) / influenceRangeS;
					;
					s.z += boid.transform.position.z * (influenceRangeS - distance) / influenceRangeS;
					;


					cs += (influenceRangeS - distance) / influenceRangeS;
				}

			}

		}

		if (cv > 0) {
			// there's influence
			v.x = v.x / cv;
			v.y = v.y / cv;
			v.z = v.z / cv;
			v.Normalize ();
			//			Debug.DrawRay (transform.position, v*5f, Color.black);
			wander = Vector3.Slerp (wander, v, 0.15f);

		}

		if (cc > 0) {


			c.x = c.x / cc;
			c.y = c.y / cc;
			c.z = c.z / cc;


			//			Debug.DrawRay (transform.position, towardCenter, Color.grey);

			lineDebug.addLine (transform.position, c);

			Vector3 towardCenter = c - transform.position;

			wander = Vector3.Slerp (wander, towardCenter, 0.25f);



		}

		if (cs > 0) {
			s.x = c.x / cs;
			s.y = c.y / cs;
			s.z = c.z / cs;

			Vector3 towardCenter = c - transform.position;

			//			Debug.DrawRay (transform.position, -1f*towardCenter, Color.green);

			wander = Vector3.Slerp (wander, -1f * towardCenter, 0.15f);

		}



	}
	// Update is called once per frame
	void Update ()
	{
		transform.position += zAxis * speed * Time.deltaTime;

		if (wandering && Random.value < wanderRate) {
			wander = Quaternion.Euler (0f, Random.Range (-1f * wanderAmplitude, wanderAmplitude), Random.Range (-1f * wanderAmplitude, wanderAmplitude)) * wander;
			//			childNull.transform.rotation = Quaternion.LookRotation(wander,Vector3.up);




			//			Debug.DrawRay (transform.position, wander*3f, Color.black);



		}
		avoidTerrain ();
		align ();


		steer2 ();


	}
}

