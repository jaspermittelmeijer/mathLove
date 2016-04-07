using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class Point
{
	// Class with methods to work with a 'point' in the delauney mesh.
	// The mesh is described in flat data, we only create these 'points' if and when we need to work with them.

	Dictionary <float,int> fovDictionary;
	List<float> fovList;
	bool inConvexMesh;
	int visibleMostForward;
	int visibleMostBackward;
	public int index;
	private Point[] connectedPoints;
	private float fov;
	static int match;
//	private int[] triangleReference;
	private Vector3[] verticeReference;
	private float PI = Mathf.PI;
	private int maxVerts;


	public Point (int _index, Vector3[] _verticeReference, int _maxVerts)
	{
		index = _index;
		verticeReference = _verticeReference;
		maxVerts = _maxVerts;

		pointSweep ();
	}


	public bool isOnEdge ()
	{
		return true;
	}

	public int getClosestPoint ()
	{
		// Return the closest point in the 'verticereference'. MAY BE OUTDATED.
		float minDistance = 100000000.0f;
		int closestPoint = -1;
		
		for (int i = 0; i < maxVerts; i++) {
			float distance = (new Vector2 (verticeReference [i].x, verticeReference [i].z) - new Vector2 (verticeReference [index].x, verticeReference [index].z)).magnitude;

			if (distance < minDistance) {
				closestPoint = i;
//				Debug.Log ("closest point: " + closestPoint);
			}
			minDistance = Mathf.Min (minDistance, distance);
		}
		return closestPoint;
	}

	private void pointSweep ()
	{
		// Method to cast a conceptual line to every other point and store those (point+angle)
		// Set up a dictionary: we use our float angle as the key, and the point that concerns as the value.

		// Establish if point is 'in mesh'. This is the case if it can see points in a range of more than 180° degrees around itself. In a delauney, the mesh is always convex, so angles will always be <180°
		// Also establish the 'most forward' and 'most backward' visible points. If outside the mesh those are the points we'll be adding new traingles to.

		fovDictionary = new Dictionary<float, int> ();
		
		Vector2 thisPoint = new Vector2 (verticeReference [index].x, verticeReference [index].z);
		
		// Loop through all points except the intended point itself
		for (int i = 0; i < maxVerts; i++) {
			if (i != index) {
				Vector2 thePoint = new Vector2 (verticeReference [i].x, verticeReference [i].z);
				Vector2 cast = thePoint - thisPoint;
				float angle = Mathf.Atan2 (cast.y, cast.x);

				if (!fovDictionary.ContainsKey (angle))
					fovDictionary.Add (angle, i);
			}
		}
		
		// Acquire keys (the angles) and sort them
		fovList = fovDictionary.Keys.ToList ();
		fovList.Sort ();
		
		// Add a key for the lowest angle plus 2PI
		fovList.Add (fovList [0] + 2 * PI);
		// Add a dictionary entry for that key, referencing the same point
		fovDictionary.Add (fovList [0] + 2 * PI, fovDictionary [fovList [0]]);

		inConvexMesh = true;
		// Loop through keys and see if there's a delta angle bigger than PI. Note that this can't happen twice. If so, this point is outside the mesh (or on the edge)
		
		for (int i = 0; i < fovList.Count - 1; i++) {
			if (fovList [i + 1] - fovList [i] > PI) {
				inConvexMesh = false;
				// The visble most forward point will be the one referenced by i+1.
				visibleMostForward = fovDictionary [fovList [i + 1]];
				visibleMostBackward = fovDictionary [fovList [i]];
			}
		}
	}

	public bool isInConvexMesh ()
	{
		return (inConvexMesh);
	}

	public int getVisibleMostForward ()
	{
		return visibleMostForward;
	}

	public int getVisibleMostBackward ()
	{
		return visibleMostBackward;
	}

	public float distanceTo (int passedIndex)
	{
		float distance = (new Vector2 (verticeReference [passedIndex].x, verticeReference [passedIndex].z) - new Vector2 (verticeReference [index].x, verticeReference [index].z)).magnitude;
		return distance;
	}
}
