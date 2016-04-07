using UnityEngine;
using System.Collections;

public class RandomTerrain
{
	
	float size, amp, roughness, unitSize;
	int arraySize, iterations;
	float[,] terraindata;
	float multiplier;

	int[] triangles;
	Vector3[] vertices;

	GameObject visualisationTarget;

	// class to generate and hold random terrain data

	public RandomTerrain (float _size, int _iterations, float _amp, float _roughness)
	{
		/*
		// http://www.gameprogrammer.com/fractal.html#diamond

		size is the pixelsize, will be a square
		iterations is the number of times we'll subdivide. So 0 means 4 cornervalues only
		amp is amplitude
		roughness defines the multiplier for amplitude with each iteration. 2 to the -H every iteration

		*/
		size = _size;

		iterations = _iterations;
		amp = _amp;
		roughness = _roughness;

		arraySize = 1 + Mathf.FloorToInt (Mathf.Pow (2.0f, iterations));

		unitSize = size / (arraySize - 1);

		visualisationTarget = GameObject.Find ("MainVisualisationObject"); // set generic debugobject

	}


	public void spawn ()
	{
		VisualisationObject.newNull (new Vector3 (0.0f, 0.0f, 0.0f), 0.3f, visualisationTarget);
		VisualisationObject.newNull (new Vector3 (0.0f, 0.0f, size), 0.3f, visualisationTarget);
		VisualisationObject.newNull (new Vector3 (size, 0.0f, 0.0f), 0.3f, visualisationTarget);
		VisualisationObject.newNull (new Vector3 (size, 0.0f, size), 0.3f, visualisationTarget);

		terraindata = new float [arraySize, arraySize];
		fillArray ();
		generate ();
		parseIntoMesh ();
//			dataDump ();
	}


	public void setVisualisationTarget (GameObject target)
	{
		visualisationTarget = target; // override the general visualisation object and set it to a specific one.
	}


	public float getHeight (float pi, float pj)
	{

		// clamp values: we'll return the height on the edge for positions outside the terrain matrix

		pi = Mathf.Clamp (pi, 0f, size);
		pj = Mathf.Clamp (pj, 0f, size);

		// return the height at this point. we'll average out neighbouring vertices in a radius of <1, weighing them based on distance.
		// establish where we are in the arraydata

		int i = Mathf.FloorToInt (pi / unitSize); // note that if a point is on the exact edge this will return a position on the edge of the array as well
		int j = Mathf.FloorToInt (pj / unitSize);
			
		// there's a max of 4 vertices that could possibly be within range that we need to consider
		average = 0;
		totalWeights = 0;



		parsePoint (pi, pj, i, j);
		parsePoint (pi, pj, i + 1, j);
		parsePoint (pi, pj, i, j + 1);
		parsePoint (pi, pj, i + 1, j + 1);



//		float theDistance = getDistanceInUnits (pi, pj, i, j);
//
//		if (theDistance < 1.0f) {
//			float weight = 1.0f - theDistance;
//
//			average += weight * terraindata [i, j];
//			totalWeights += weight;
//
//		}
//






		if (totalWeights > 0) {
			return average / totalWeights;
		} else {
			return -1000.0f;
		}

	}

	float average, totalWeights;

	private void parsePoint (float pi, float pj, int ii, int jj)
	{
		float theDistance = getDistanceInUnits (pi, pj, ii, jj);

//		Debug.Log ("Distance in units to  point " +ii+" "+jj+" : " + getDistanceInUnits (pi, pj, ii, jj));

		if (theDistance < 1.0f) {
			float weight = 1.0f - theDistance;

			average += weight * terraindata [ii, jj];
			totalWeights += weight;

		}

	}

	public float getDistanceInUnits (float pi, float pj, int ii, int jj)
	{

		// check if ii and jj are valid: they may go out of range

		if (ii >= 0 && ii < arraySize && jj >= 0 && jj < arraySize) {

			float distance = Vector2.Distance (new Vector2 (pi, pj), new Vector2 (ii * unitSize, jj * unitSize));
			distance = distance / unitSize;

			return distance;
		} else {
			return 1000.0f;
		}

	}



	private void parseIntoMesh ()
	{
		vertices = new Vector3[arraySize * arraySize];
		triangles = new int[(arraySize - 1) * (arraySize - 1) * 2 * 3];


		// Go over the entire array and parse it into vertices and triangles.
		for (int i = 0; i < arraySize; i++) {
			for (int j = 0; j < arraySize; j++) {
				int currentVerticeIndex = i * arraySize + j;
				int currentTriangleIndex = i * (arraySize - 1) + j; 

				// Add vertice.
				vertices [currentVerticeIndex] = new Vector3 (i * unitSize, terraindata [i, j], j * unitSize);

				// Add 2 triangles for every square. So that's for every vertice except the last one in a row or column.
				if (i < arraySize - 1 && j < arraySize - 1) {
					triangles [6 * currentTriangleIndex + 0] = currentVerticeIndex;
					triangles [6 * currentTriangleIndex + 1] = currentVerticeIndex + 1;
					triangles [6 * currentTriangleIndex + 2] = currentVerticeIndex + arraySize;

					triangles [6 * currentTriangleIndex + 3] = currentVerticeIndex + 1;
					triangles [6 * currentTriangleIndex + 4] = currentVerticeIndex + arraySize + 1;
					triangles [6 * currentTriangleIndex + 5] = currentVerticeIndex + arraySize;


				}


			}

		}



	}

	public Vector3[] getVertices ()
	{

		return vertices;

	}

	public int[] getTriangles ()
	{

		return triangles;

	}



	public void generate ()
	{
		// corner values
		terraindata [0, 0] = Random.Range (-1.0f, 1.0f) * amp;
		terraindata [0, arraySize - 1] = Random.Range (-1.0f, 1.0f) * amp;
		terraindata [arraySize - 1, 0] = Random.Range (-1.0f, 1.0f) * amp;
		terraindata [arraySize - 1, arraySize - 1] = Random.Range (-1.0f, 1.0f) * amp;

//		terraindata [0, 0] = 0.0f;
//		terraindata [0, size - 1] = 0.0f;
//		terraindata [size - 1, 0] = 1000.0f;
//		terraindata [size - 1, size - 1] = 1000.0f;

		// 
	
					
	

		// now iterate at increasingly deeper levels.
		multiplier = Mathf.Pow (2.0f, -1.0f * roughness);
//		multiplier = 1f;

		int iteration = 0;

		while (iteration < iterations) {

			// Step defines the size of the current square in this iteration.
			int step = Mathf.FloorToInt (Mathf.Pow (2, iterations - iteration));




			for (int i = 0; i < arraySize - 1; i += step) {
				for (int j = 0; j < arraySize - 1; j += step) {
					diamond (i, j, step);
					                


				}
			}

			for (int i = 0; i < arraySize - 1; i += step) {
				for (int j = 0; j < arraySize - 1; j += step) {
//					diamond (ix * currentSize, iy * currentSize, currentLevel);
					int mid = step / 2;
					// we'll move in a circle over the square's sides
					float average;

					average = terraindata [i, j] + terraindata [i + step, j];
					terraindata [i + mid, j] = average / 2.0f + Random.Range (-1, 1) * amp * multiplier; 

					average = terraindata [i + step, j] + terraindata [i + step, j + step];
					terraindata [i + step, j + mid] = average / 2.0f + Random.Range (-1, 1) * amp * multiplier; 

					average = terraindata [i + step, j + step] + terraindata [i, j + step];
					terraindata [i + mid, j + step] = average / 2.0f + Random.Range (-1, 1) * amp * multiplier; 

					average = terraindata [i, j + step] + terraindata [i, j];
					terraindata [i, j + mid] = average / 2.0f + Random.Range (-1, 1) * amp * multiplier; 
								}
			}

			multiplier = multiplier * Mathf.Pow (2.0f, -1.0f * roughness);

			iteration++;
		}
			}

	private void fillArray ()
	{
		for (int i = 0; i < arraySize; i++) {
			for (int j = 0; j < arraySize; j++) {
				terraindata [i, j] = -1000.0f;
			}
		}
	}

	public void dataDump ()
	{

		for (int i = 0; i < arraySize; i++) {
			for (int j = 0; j < arraySize; j++) {
				VisualisationObject.newPointDiamond (new Vector3 ((float)i / (arraySize - 1) * size, terraindata [i, j], (float)j / (arraySize - 1) * size), new Color (1.0f, 1.0f, 1.0f), 0.1f, visualisationTarget);
			}
		}
	}

	private void diamond (int i, int j, int step)
	{
		// plot a new value into the heart of a square
				int mid = step / 2;
		float average = terraindata [i, j] + terraindata [i, j + step] + terraindata [i + step, j] + terraindata [i + step, j + step];
		terraindata [i + mid, j + mid] = (average / 4.0f) + Random.Range (-1, 1) * amp * multiplier;
	}
	}
