using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;

public class GeometryPlayer : MonoBehaviour
{


	ArrayList meshPlaylist;
	float timer, duration;
	int index;
	ExtendedMesh local;
	ArrayList durations;
	ArrayList gameObjectReferences;


	// Use this for initialization
	void Start ()
	{
		meshPlaylist = new ArrayList ();
		durations = new ArrayList ();
		gameObjectReferences = new ArrayList ();
		timer = 0f;
		duration = 1f;
		index = 0;
		frames = 0;

//		end = 125;

	}


	/*
	// Update is called once per frame
	void Update () {
		
	
		if (index < meshPlaylist.Count) {
			timer += Time.deltaTime;

			if (timer > duration) {
				
				while (timer > duration) { // skip frames
					timer -= duration;


					local = (ExtendedMesh)meshPlaylist [index];
					duration = local.getDuration ();
					index++;
					if (index == meshPlaylist.Count) {
						break;

					}

				}

//				transform.gameObject.GetComponent <MeshFilter> ().mesh = local.getMesh ();
//				GetComponent <CustomRender>().CreateLinesFromMesh ();
				Debug.Log (index);
			}
		}
			
	}	


	// Update is called once per frame
	void Update () {


		if (index < durations.Count) {
			timer += Time.deltaTime;

			if (timer > duration) {

				while (timer > duration) { // skip frames
					timer -= duration;

					GameObject local = (GameObject) gameObjectReferences [index];
					local.SetActive (false);

//					local = (ExtendedMesh)meshPlaylist [index];
					duration = (float) durations [index];
					index++;
					if (index == durations.Count) {
						break;

					}

				}
				GameObject theObject = (GameObject) gameObjectReferences [index];
				theObject.SetActive (true);


				//				transform.gameObject.GetComponent <MeshFilter> ().mesh = local.getMesh ();
				//				GetComponent <CustomRender>().CreateLinesFromMesh ();
				Debug.Log (index);
			}
		}

	
	}


	*/

	int frames;

	// Update is called once per frame
	void Update ()
	{

		frames++;
		if (frames == 2) {
			frames = 0;

			if (index < durations.Count - 1) {
//				if (index < 30) {
					
				transform.GetChild (index).gameObject.SetActive (false);

//
//				GameObject theObject = (GameObject)gameObjectReferences [index];
//				theObject.SetActive (false);

				index++;

				transform.GetChild (index).gameObject.SetActive (true);



//			 theObject = (GameObject)gameObjectReferences [index];
//			theObject.SetActive (true);


			}
		}

	}


	public void addMesh (ExtendedMesh passMesh)
	{

//		meshPlaylist.Add (passMesh); // 

		durations.Add (passMesh.getDuration ());


//		Debug.Log ("Mesh added to playlist");

		GameObject workingObject = new GameObject ("Frame");
		workingObject.transform.parent = transform;

		workingObject.AddComponent<MeshFilter> ();
		workingObject.AddComponent<MeshRenderer> ();
		workingObject.AddComponent<CustomRender> ();

		workingObject.GetComponent<MeshFilter> ().mesh = passMesh.getMesh ();
		workingObject.GetComponent <CustomRender> ().CreateLinesFromMesh ();
		workingObject.GetComponent<CustomRender> ().passColor (GameObject.Find ("Root").GetComponent <Settings> ().lineColor01);

		workingObject.GetComponent<Renderer> ().material = Resources.Load ("Default") as Material;
		workingObject.GetComponent<Renderer> ().useLightProbes = false;
		workingObject.GetComponent<Renderer> ().reflectionProbeUsage = ReflectionProbeUsage.Off;

		workingObject.GetComponent<Renderer> ().shadowCastingMode = ShadowCastingMode.TwoSided;
		workingObject.GetComponent<Renderer> ().receiveShadows = true;





		workingObject.SetActive (false);

		gameObjectReferences.Add (workingObject);


	}


}
