using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace Completed
{
	using System.Collections.Generic;		//Allows us to use Lists. 
	using UnityEngine.UI;					//Allows us to use UI.
	
	public class GameManager : MonoBehaviour
	{
		public float turnDelay = 0.1f;							//Delay between each Player turn.
		public int healthPoints = 100;							//Starting value for Player health points.
		public static GameManager instance = null;				//Static instance of GameManager which allows it to be accessed by any other script.
		[HideInInspector] public bool playersTurn = true;       //Boolean to check if it's players turn, hidden in inspector but public.

        private BoardManager boardScript;                       //Store a reference to our BoardManager which will set up the level.
        private List<Enemy> enemies;                            //List of all Enemy units, used to issue them move commands.
        private bool enemiesMoving;								//Boolean to check if enemies are moving.

        private Text levelText;									//Text to display current level number.
		private GameObject levelImage;							//Image to block out level as levels are being set up, background for levelText.
		private int level = 1;									//Current level number, expressed in game as "Day 1".
		private bool doingSetup = true;                         //Boolean to check if we're setting up board, prevent Player from moving during setup.

		//Awake is always called before any Start functions
		void Awake()
		{
			//Check if instance already exists
			if (instance == null)
			{
				//if not, set instance to this
				instance = this;
			}

			//If instance already exists and it's not this:
			else if (instance != this)
			{
				//Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
				Destroy(gameObject);
            }

            //Sets this to not be destroyed when reloading scene
            DontDestroyOnLoad(gameObject);
			
			//Assign enemies to a new List of Enemy objects.
			enemies = new List<Enemy>();
			
			//Get a component reference to the attached BoardManager script
			boardScript = GetComponent<BoardManager>();
			
			//Call the InitGame function to initialize the first level 
			InitGame();
		}
		
		//Initializes the game for each level.
		void InitGame()
		{
			enemies.Clear();

			boardScript.BoardSetup();
		}
		
		public void UpdateBoard(int horizontal, int vertical)
		{
        }
	}
}

