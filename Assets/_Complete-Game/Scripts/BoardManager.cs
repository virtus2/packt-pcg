using UnityEngine;
using System;
using System.Collections.Generic; 		//Allows us to use Lists.
using Random = UnityEngine.Random; 		//Tells Random to use the Unity Engine random number generator.

namespace Completed
	
{
	
	public class BoardManager : MonoBehaviour
	{
		// Using Serializable allows us to embed a class with sub properties in the inspector.
		[Serializable]
		public class Count
		{
			public int minimum; 			//Minimum value for our Count class.
			public int maximum; 			//Maximum value for our Count class.
			
			//Assignment constructor.
			public Count (int min, int max)
			{
				minimum = min;
				maximum = max;
			}
		}
		
		public int columns = 5; 										//Number of columns in our game board.
		public int rows = 5;											//Number of rows in our game board.
		public GameObject[] floorTiles;									//Array of floor prefabs.
		private Transform boardHolder;                                  //A variable to store a reference to the transform of our Board object.
		private Dictionary<Vector2, Vector2> gridPositions = new Dictionary<Vector2, Vector2>();
		
		//Sets up the outer walls and floor (background) of the game board.
		public void BoardSetup ()
		{
			//Instantiate Board and set boardHolder to its transform.
			boardHolder = new GameObject ("Board").transform;
			
			for(int x=0; x < columns; x++)
			{
				for(int y=0; y < rows; y++)
				{
					gridPositions.Add(new Vector2(x, y), new Vector2(x, y));

					GameObject toInstantiate = floorTiles[Random.Range (0,floorTiles.Length)];
					GameObject instance = Instantiate (toInstantiate, new Vector3 (x, y, 0f), Quaternion.identity) as GameObject;
					instance.transform.SetParent(boardHolder);
                }
			}
		}
	}
}
