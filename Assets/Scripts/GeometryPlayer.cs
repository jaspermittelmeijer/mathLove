﻿using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;

public class GeometryPlayer 
{


	ArrayList meshPlaylist;
	float timer, duration;
	int index;
	ExtendedMesh local;
	ArrayList durations;
	ArrayList gameObjectReferences;
	GameObject target;


	public GeometryPlayer (GameObject passTarget){
		target = passTarget;
		meshPlaylist = new ArrayList ();
		durations = new ArrayList ();
		gameObjectReferences = new ArrayList ();
		timer = 0f;
		duration = 1f;
		index = 0;
		frames = 0;
	}




	int frames;

	// Update is called once per frame
	public void update ()
	{

		frames++;
		if (frames == 2) {
			frames = 0;

			if (index < durations.Count - 1) {
//				if (index < 30) {
					
				target.transform.GetChild (index).gameObject.SetActive (false);

//
//			GameObject theObject = (GameObject)gameObjectReferences [index];
//				theObject.SetActive (false);

				index++;

				target.transform.GetChild (index).gameObject.SetActive (true);



//			 theObject = (GameObject)gameObjectReferences [index];
//			theObject.SetActive (true);


			} else {
				target.transform.GetChild (index).gameObject.SetActive (false);
				index = 0;
				target.transform.GetChild (index).gameObject.SetActive (true);
			}
		}

	}


	public void addMesh (ExtendedMesh passMesh)
	{

//		meshPlaylist.Add (passMesh); // 

		durations.Add (passMesh.getDuration ());


//		Debug.Log ("Mesh added to playlist");

		GameObject workingObject = new GameObject ("Frame");
		workingObject.transform.parent = target.transform;

		workingObject.AddComponent<MeshFilter> ();
		workingObject.AddComponent<MeshRenderer> ();
		workingObject.AddComponent<CustomRender> ();

		workingObject.GetComponent<MeshFilter> ().mesh = passMesh.getMesh ();
		workingObject.GetComponent <CustomRender> ().CreateLinesFromMesh ();
		workingObject.GetComponent<CustomRender> ().passColor (GameObject.Find ("Root").GetComponent <MLSettings> ().lineColor01);

		workingObject.GetComponent<Renderer> ().material = Resources.Load ("Default") as Material;
		workingObject.GetComponent<Renderer> ().useLightProbes = false;
		workingObject.GetComponent<Renderer> ().reflectionProbeUsage = ReflectionProbeUsage.Off;

//		workingObject.GetComponent<Renderer> ().shadowCastingMode = ShadowCastingMode.TwoSided;
		workingObject.GetComponent<Renderer> ().receiveShadows = true;





		workingObject.SetActive (false);

		gameObjectReferences.Add (workingObject);


	}


}
