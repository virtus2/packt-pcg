using UnityEngine;
using System.Collections;

using System.Collections.Generic;
using UnityEngine.UI;
using Completed;

public class GameManager : MonoBehaviour
{
    public float turnDelay = 0.1f;
    public int healthPoints = 100;
    public static GameManager instance = null;
    [HideInInspector] public bool playersTurn = true;

    public int enemySpawnRatio = 20;

    private BoardManager boardScript;
    private DungeonManager dungeonScript;
    private Player playerScript;
    private List<Enemy> enemies;
    private bool enemiesMoving;

    private bool playerInDungeon;

    //Awake is always called before any Start functions
    void Awake()
    {
        if (instance == null)
            instance = this;

        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        enemies = new List<Enemy>();

        boardScript = GetComponent<BoardManager>();
        dungeonScript = GetComponent<DungeonManager>();
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        InitGame();
    }

    //This is called each time a scene is loaded.
    void OnLevelWasLoaded(int index)
    {
        //Call InitGame to initialize our level.
        InitGame();
    }

    //Initializes the game for each level.
    void InitGame()
    {
        enemies.Clear();
        boardScript.BoardSetup();
        playerInDungeon = false;
    }

    //Update is called every frame.
    void Update()
    {
        //Check that playersTurn or enemiesMoving or doingSetup are not currently true.
        if (playersTurn || enemiesMoving)

            //If any of these are true, return and do not start MoveEnemies.
            return;

        //Start moving enemies.
        StartCoroutine(MoveEnemies());
    }

    //GameOver is called when the player reaches 0 health points
    public void GameOver()
    {
        //Disable this GameManager.
        enabled = false;
    }

    //Coroutine to move enemies in sequence.
    IEnumerator MoveEnemies()
    {
        enemiesMoving = true;

        yield return new WaitForSeconds(turnDelay);

        if (enemies.Count == 0)
        {
            yield return new WaitForSeconds(turnDelay);
        }
        List<Enemy> enemiesToDestroy = new List<Enemy>();

        for (int i = 0; i < enemies.Count; i++)
        {
            if (playerInDungeon)
            {
                if (!enemies[i].getSpriteRenderer().isVisible)
                {
                    if (i == enemies.Count - 1)
                        yield return new WaitForSeconds(enemies[i].moveTime);
                    continue;
                }
            }
            else
            {
                if ((!enemies[i].getSpriteRenderer().isVisible) || (!boardScript.checkValidTile(enemies[i].transform.position)))
                {
                    enemiesToDestroy.Add(enemies[i]);
                    continue;
                }
            }

            enemies[i].MoveEnemy();

            yield return new WaitForSeconds(enemies[i].moveTime);
        }
        playersTurn = true;

        enemiesMoving = false;

        for (int i = 0; i < enemiesToDestroy.Count; i++)
        {
            enemies.Remove(enemiesToDestroy[i]);
            Destroy(enemiesToDestroy[i].gameObject);
        }
        enemiesToDestroy.Clear();
    }

    public void updateBoard(int horizantal, int vertical)
    {
        boardScript.addToBoard(horizantal, vertical);
    }

    public void enterDungeon()
    {
        dungeonScript.StartDungeon();
        boardScript.SetDungeonBoard(dungeonScript.gridPositions, dungeonScript.maxBound, dungeonScript.endPos);
        playerScript.dungeonTransition = false;
        playerInDungeon = true;
        for(int i=0; i<enemies.Count; i++)
        {
            Destroy(enemies[i].gameObject);
        }
        enemies.Clear();
    }

    public void exitDungeon()
    {
        boardScript.SetWorldBoard();
        playerScript.dungeonTransition = false;
        playerInDungeon = false;
        enemies.Clear();
    }

    public void AddEnemyToList(Enemy enemy)
    {
        enemies.Add(enemy);
    }

    public void RemoveEnemyFromList(Enemy enemy)
    {
        enemies.Remove(enemy);
    }
}