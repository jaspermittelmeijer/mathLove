
using UnityEngine;
using System.Collections;

static public class Triangle
{
	// Static class to work with triangles

	static int[] ind;
	static Vector2 pa, pb, pc, pd;
	static  int[] connectedTriangles;

	static public int[] getConnectedTrianglesFor (int triangleIndex, ref int[] triangleData)
	{
		// get connected triangles for the triangle at the passed index

		ind = new int[3];

		ind [0] = triangleData [triangleIndex * 3 + 0];
		ind [1] = triangleData [triangleIndex * 3 + 1];
		ind [2] = triangleData [triangleIndex * 3 + 2];

		// Max 3. We look for the same points. If any set of 3 has 2 in common they share that edge.
		int[] connectedTriangles = new int[3];
		connectedTriangles [0] = -1;
		connectedTriangles [1] = -1;
		connectedTriangles [2] = -1;
			
		int i = 0;
		int t = 0;
		while (i < triangleData.Length) {
		
			int common = 0;
			for (int i2 = 0; i2 < 3; i2++) {

				for (int j = 0; j < 3; j++) {
					if (triangleData [i + i2] == ind [j]) {
						common++;
					}
				}
			}
			if (common == 2) { // if 3 points in common it's this triangle
				connectedTriangles [t] = i / 3;
				t++;
			}
			i += 3;
		}

		return (connectedTriangles);
	}

	static public bool pointWithinBoundsOf (int pointIndex, int triangleIndex, ref Vector3[] verticeData, ref int[] triangleData)
	{
		// Is point at pointindex within the bounds of the traingle at triangleIndex?

		ind [0] = triangleData [triangleIndex * 3 + 0];
		ind [1] = triangleData [triangleIndex * 3 + 1];
		ind [2] = triangleData [triangleIndex * 3 + 2];

		pd = new Vector2 (verticeData [pointIndex].x, verticeData [pointIndex].z);
		pa = new Vector2 (verticeData [ind [0]].x, verticeData [ind [0]].z);
		pb = new Vector2 (verticeData [ind [1]].x, verticeData [ind [1]].z);
		pc = new Vector2 (verticeData [ind [2]].x, verticeData [ind [2]].z);


		// A point is within this triangle if, while iterating anti-clockwise, it is always clockwise, or vice versa
		// NOTE: current code may fail if normal up or down??

		bool bool01 = vectorsClockwise (pb - pa, pd - pa);
		bool bool02 = vectorsClockwise (pc - pb, pd - pb);
		bool bool03 = vectorsClockwise (pa - pc, pd - pc);

		// Check if ac is clockwise from ab or not

		if (vectorsClockwise (pb - pa, pc - pa)) {
			if (bool01 && bool02 && bool03)
				return true;
			else
				return false;
			
		} else {
					
			if (bool01 && bool02 && bool03)
				return false;
			else
				return true;
		}
	}

	static	bool vectorsClockwise (Vector2 vec1, Vector2 vec2)
	{
		float angle1 = Mathf.Atan2 (vec1.y, vec1.x); // -pi < atan2 <= pi
		float angle2 = Mathf.Atan2 (vec2.y, vec2.x); // 

		float angleDelta = angle1 - angle2; // E -2PI < ad <= 2PI
		// if <0 we add 2PI to simplify things
		if (angleDelta < 0.0f)
			angleDelta += 2.0f * Mathf.PI;
	
		// 0..pi:clockwise, pi..2pi: anticlock
		if (0.0f < angleDelta && angleDelta <= Mathf.PI) {
			return true;
		} else {
			return false;
		}
	}
}
