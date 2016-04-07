using UnityEngine;
using System.Collections.Generic;

public class CustomRender : MonoBehaviour
{
	// Class to apply line rendering to an object.

	public Material lineMaterial;
	private List<int> lines;
	private Vector3[] vertices;
	public Color lineColor = Color.black;


	void Start ()
	{
				CreateLinesFromMesh ();
			}
	public void passColor (Color theColor) {
				lineColor = theColor;
	}

	private bool lineExists (int a, int b)
	{
		bool returnValue = false;
	
		// find occurences of a at even positions
		int i = 0;
		while (i < lines.Count) {
			int j = lines.IndexOf (a,i);

			if ((j) == -1){
				break;
			} else 
			{
				// if at even position and next position is b, than we have a duplicate
			   if ((j % 2 ==0) && (lines[j+1]==b)) {
					returnValue = true;
//					Debug.Log ("Duplicate found");
					break;
				}
			}
			i=j+1;
		}
		return returnValue;
	}

	private void addLine (int a, int b)
	{
		// Always store lines with the lower vertice first
		if (a > b) {
			int c = a;
			a = b;
			b = c;
		}

		if (!lineExists (a, b)) {
			lines.Add (a);
			lines.Add (b);
		}
	}

	public bool CreateLinesFromMesh ()
	{
		// this will create an array of lines from mesh data

		// get a reference to our mesh
		Mesh mesh = GetComponent<MeshFilter> ().mesh;
		vertices = mesh.vertices;
		int[] triangles = mesh.triangles;

		// Create a list of ints to describe lines
		lines = new List<int> ();

		// Iterate through all triangles to add lines

		for (int i=0; i<triangles.Length; i+=3) {
			addLine (triangles [i + 0], triangles [i + 1]);
			addLine (triangles [i + 0], triangles [i + 2]);
			addLine (triangles [i + 1], triangles [i + 2]);

		}

		return true;
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


			GL.Vertex3 (vertices [lines [i]].x, vertices [lines [i]].y, vertices [lines [i]].z);
			GL.Vertex3 (vertices [lines [i + 1]].x, vertices [lines [i + 1]].y, vertices [lines [i + 1]].z);

		}



		GL.End ();
		GL.PopMatrix ();
	}
}