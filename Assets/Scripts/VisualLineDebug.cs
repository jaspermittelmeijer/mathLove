using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class VisualLineDebug : MonoBehaviour {


	private List<Vector3> lines;
	public Material lineMaterial;
	public Color lineColor = Color.grey;

	// Use this for initialization
	void Start () {
		lines = new List<Vector3>();

	}
	
	// Update is called once per frame
	void Update () {
		lines = new List<Vector3>();
	}
	private void CreateLineMaterial ()
	{
		if (!lineMaterial) {
			// Unity has a built-in shader that is useful for drawing
			// simple colored things.
			//			Shader shader = Shader.Find ("Hidden/Internal-Colored");

			//			Material material = Material.find
			//			Shader shader = Shader.Find ("Standard");
			Shader shader = Shader.Find ("Hidden/Internal-Colored");


			lineMaterial = new Material (shader);

			lineMaterial.hideFlags = HideFlags.HideAndDontSave;
			// Turn on alpha blending
			lineMaterial.SetInt ("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
			lineMaterial.SetInt ("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
			// Turn backface culling off
			lineMaterial.SetInt ("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
			// Turn off depth writes
			lineMaterial.SetInt ("_ZWrite", 0);
		}
	}

	public void addLine (Vector3 a, Vector3 b){
		lines.Add (a);
		lines.Add (b);
	}


	// Will be called after all regular rendering is done
	public void OnRenderObject ()
	{
		CreateLineMaterial ();

		// Apply the line material
		lineMaterial.SetPass (0);

		GL.PushMatrix ();
		// Set transformation matrix for drawing to
		// match our transform
		GL.MultMatrix (transform.localToWorldMatrix);

		// Draw lines
		GL.Begin (GL.LINES);

		// Cycle through the list of lines
		for (int i = 0; i < lines.Count; i+=2) {
			//			GL.Color (Color.red);
			GL.Color (lineColor);


			GL.Vertex3 (lines[i].x,lines[i].y,lines[i].z);
			GL.Vertex3 (lines[i+1].x,lines[i+1].y,lines[i+1].z);

		}



		GL.End ();
		GL.PopMatrix ();

		// 

	}


}

