using UnityEngine;
using System.Collections;


public delegate void DelauneyStateHandler (Vector3[] vertices, int[] triangles);

public class DelauneyAlgorithm
{
	// Runs the algorithm, generating mesh states as it goes and calls a delegate on every state.
	// The execution should be called as a background task / coroutine

	int vertices;
	float dimensions;
	RandomTerrain terrain;

	bool processAllStates ;
	DelauneyStateHandler theStateHandler;


	Vector3[] verticeData;
	int[] triangleData;
	int triangleIndex;
	ArrayList flipStack;


	public DelauneyAlgorithm (int _vertices, float _dimensions, RandomTerrain _terrain, bool _processAll, DelauneyStateHandler _handler)
	{
		vertices = _vertices;
		dimensions = _dimensions;
		terrain = _terrain;
		theStateHandler = _handler;
		processAllStates = _processAll;
	}

	public IEnumerator process (TaskStatus theStatus)
	{
		verticeData = new Vector3[vertices + 1];

		// first 3 verices = 1 triangle. Each next vertice can generate no more than 2 additional triangles. This may be untrue
		triangleData = new int[3 * vertices * 2 ];

		for (int i = 0; i < triangleData.Length; i++)
			triangleData [i] = vertices;

		float x, y, z;

		for (int i = 0; i < 3; i++) {
			x = Random.value * dimensions;
			z = Random.value * dimensions;
			y = terrain.getHeight (x, z);

			verticeData [i] = new Vector3 (x, y, z);
		}

		// Set up the initial triangle for delauney
		addTriangle (0, 1, 2, 0);

		// Now start adding points, and add triangles as we go

		int n = 3;
		triangleIndex = 1;
		flipStack = new ArrayList ();

		while (n < vertices) {

			// Create a vertice and store it.
			x = Random.value * dimensions;
			z = Random.value * dimensions;
			y = terrain.getHeight (x, z);

			verticeData [n] = new Vector3 (x, y, z);

			// Create point for our new vertice
			Point newPoint = new Point (n, verticeData, n);

			// Check if the point is in the mesh. If it's field of view is smaller than 180°, it is outside. If it is larger than 180°, it is inside.
			// Next, we could reject a point if it's fov is too close to 180, which'd mean it was very close to the edge.

			if (newPoint.isInConvexMesh ()) {
				// Point is in the existing mesh.				
				// We'll need to find the triangle it is in, delete that and split it into 3 new ones.

				for (int ti = 0; ti < triangleIndex; ti++) {

					if (Triangle.pointWithinBoundsOf (n, ti, ref verticeData, ref triangleData)) {
						int ti0 = triangleData [ti * 3 + 0];
						int ti1 = triangleData [ti * 3 + 1];
						int ti2 = triangleData [ti * 3 + 2];

						// We found the triangle our point is in. Now we need to delete that triangle and create 3 new ones. We'll add those triangles to the flipstack, for flipping into delauney triangulation.

						addTriangle (ti0, ti1, n, ti); // replace the triangle the new point is in

						addToFlipStack (ti);

						addTriangle (ti1, ti2, n, triangleIndex); // add a triangle
						addToFlipStack (triangleIndex);
						triangleIndex++;

						addTriangle (ti2, ti0, n, triangleIndex); // add another triangle
						addToFlipStack (triangleIndex);
						triangleIndex++;

						break; // we found our triangle and can break the loop
					} 
				}

				if (processAllStates ) {
					processState ();

					yield return null;
				}

			} else {
				// Point is not in existing mesh. Which means we'll add triangles to connect it to all the vertices it can 'see'.

				int current, next, end;

				// Find out what 'visible' point is most anticlockwise, we'll start there.
				current = newPoint.getVisibleMostBackward ();

				// Find out what 'visible' point is most clockwise, we'll stop there.
				end = newPoint.getVisibleMostForward ();

				// Create our departure point. This point is on the edge by definition, since it is the outer one 'visible' to our new point.

				while (current != end) {
					Point currentPoint = new Point (current, verticeData, n);
					next = currentPoint.getVisibleMostForward ();

					addTriangle (current, next, n, triangleIndex);
					addToFlipStack (triangleIndex);

					if (processAllStates ) {
						processState ();

						yield return null;
					}

					triangleIndex++;
					current = next;
				}

			}
			n++;

			while (flipStack.Count > 0) {
				if (flipFlipStack ()) {
					if (processAllStates ) {
						processState ();

						yield return null;
					}
				}

			}

		}


		if (!processAllStates ) {
			processState ();

			yield return null;
		}

		theStatus.isDone = true;

	}

	// ***************************************************************************************************
	// Flipping methods
	// ***************************************************************************************************

	bool delauneyTest (int a, int b)
	{

		// test if touching triangles a and b are delauney: the inner angles opposite both sides of the common edge, when added, cannot be more than 180°
		// we need to the 2 vertices that do not appear in the other triangle
		int[] vertsA = new int[3];
		int[] vertsB = new int[3];

		vertsA [0] = triangleData [a * 3 + 0];
		vertsA [1] = triangleData [a * 3 + 1];
		vertsA [2] = triangleData [a * 3 + 2];

		vertsB [0] = triangleData [b * 3 + 0];
		vertsB [1] = triangleData [b * 3 + 1];
		vertsB [2] = triangleData [b * 3 + 2];

		int va, vb, vc, vd; // a and b are the uncommon vertices, c and d are the common edge

		va = -1;
		vb = -1;
		vc = -1;
		vd = -1;
		for (int i = 0; i < 3; i++) {
			bool isEdge = false;
			for (int j = 0; j < 3; j++) {
				if (vertsA [i] == vertsB [j]) {

					isEdge = true;

				}
			}
			if (!isEdge) {
				va = vertsA [i];


			}
			if (isEdge) {
				if (vc == -1)
					vc = vertsA [i];
				else
					vd = vertsA [i];

			}
		}

		for (int i = 0; i < 3; i++) {
			bool isEdge = false;
			for (int j = 0; j < 3; j++) {
				if (vertsB [i] == vertsA [j]) {

					isEdge = true;

				}
			}
			if (!isEdge) {
				vb = vertsB [i];
			}
		}

		Vector3 pa, pb, pc, pd;

		pa = new Vector2 (verticeData [va].x, verticeData [va].z);
		pb = new Vector2 (verticeData [vb].x, verticeData [vb].z);
		pc = new Vector2 (verticeData [vc].x, verticeData [vc].z);
		pd = new Vector2 (verticeData [vd].x, verticeData [vd].z);

		float anglea = Vector2.Angle (pc - pa, pd - pa);
		float angleb = Vector2.Angle (pc - pb, pd - pb);

		if (anglea + angleb > 180.0f) {
			addTriangle (va, vb, vc, a);
			addTriangle (va, vb, vd, b);
			return true;

		} else {

			return false;
		}
	}



	private void addToFlipStack (int a)
	{
		if (flipStack.IndexOf (a) == -1) {
			flipStack.Add (a);
		}
	}


	bool flipFlipStack ()
	{
		bool flipped = false;

		int i = (int)flipStack [0];

		int[] getConnected = Triangle.getConnectedTrianglesFor (i, ref triangleData);
		flipStack.RemoveAt (0);

		for (int j = 0; j < 3; j++) {
			if (getConnected [j] != -1) {
				// Once we have flipped triangles, it's useless to look at the other connected triangles, since our original has now changed and we don't even now if they're still connected. 
				// That does however mean we need to stack those connected triangles because they may still be off and we wouldn't touch on them otherwise.

				if (!flipped) {

					if (delauneyTest (i, getConnected [j])) {
						flipped = true;
						// flipping the triangles may affect others
						addToFlipStack (i);
						addToFlipStack (j);

					} 
				} else {
					// we have an orphaned triangle that we need to restack
					addToFlipStack (getConnected [j]);
				}
			}
		}

		return flipped;
	}

	// ***************************************************************************************************
	// Geometry methods
	// ***************************************************************************************************

	void processState () {
		
		theStateHandler (getUniqueVertices (),getUniqueTriangles ()); // call our delegate to do as it pleases with this data

	}



	void addTriangle (int a, int b, int c, int i)
	{
		Vector3 leg1, leg2, normal;

		leg1 = verticeData [b] - verticeData [a];
		leg2 = verticeData [c] - verticeData [a];
		normal = Vector3.Cross (leg1, leg2);

		if (normal.y >= 0.0f) {
			triangleData [i * 3 + 0] = a;
			triangleData [i * 3 + 1] = b;
			triangleData [i * 3 + 2] = c;

		} else {

			triangleData [i * 3 + 0] = a;
			triangleData [i * 3 + 1] = c;
			triangleData [i * 3 + 2] = b;
		}
	}

	Vector3[] uniqueVertices;
	int[] uniqueTriangles;


	public Vector3[] getUniqueVertices ()
	{

		uniqueVertices = new Vector3[triangleData.Length];
		uniqueTriangles = new int[triangleData.Length];

		// we'll have the same nr of vertices as are referenced in triangledata , after all: we are building unique ones. indices in new triangle data will go up 0.1.2.3.etc

		for (int t = 0; t < triangleData.Length; t += 1) {
			// go through all triangle-points, and create unique vertices for each

			uniqueVertices [t] = verticeData [triangleData [t]];
			uniqueTriangles [t] = t;
		}

		return uniqueVertices;
	}

	public int[] getUniqueTriangles ()
	{
		// check if uniquevertices have already been calculated...
		if (uniqueVertices == null) {
			getUniqueVertices ();
		}
		return uniqueTriangles;

	}




}

