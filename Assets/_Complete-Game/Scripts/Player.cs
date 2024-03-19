using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;	//Allows us to use UI.

//Player inherits from MovingObject, our base class for objects that can move, Enemy also inherits from this.
public class Player : MovingObject
{
    public static bool isFacingRight;
    public int wallDamage = 1;                  //How much damage a player does to a wall when chopping it.
    public Text healthText;                     //UI Text to display current player health total.
    private Animator animator;                  //Used to store a reference to the Player's animator component.
    private int health;                         //Used to store player health points total during level.
    public static Vector2 position;

    public bool onWorldBoard;
    public bool dungeonTransition;

    private Weapon weapon;
    public Image weaponComp1, weaponComp2, weaponComp3;
    public Image gloves;
    public Image boot;

    public int attackMod = 0, defenseMod = 0;
    private Dictionary<string, Item> inventory;

    protected override void Start()
    {
        animator = GetComponent<Animator>();

        health = GameManager.instance.healthPoints;

        healthText.text = "Health: " + health;

        position.x = position.y = 2;

        onWorldBoard = true;
        dungeonTransition = false;

        inventory = new Dictionary<string, Item>();

        base.Start();
    }

    private void Update()
    {
        //If it's not the player's turn, exit the function.
        if (!GameManager.instance.playersTurn) return;

        int horizontal = 0;     //Used to store the horizontal move direction.
        int vertical = 0;       //Used to store the vertical move direction.

        bool canMove = false;

        //Get input from the input manager, round it to an integer and store in horizontal to set x axis move direction
        horizontal = (int)(Input.GetAxisRaw("Horizontal"));

        //Get input from the input manager, round it to an integer and store in vertical to set y axis move direction
        vertical = (int)(Input.GetAxisRaw("Vertical"));

        //Check if moving horizontally, if so set vertical to zero.
        if (horizontal != 0)
        {
            vertical = 0;
        }

        if (horizontal != 0 || vertical != 0)
        {
            if(!dungeonTransition)
            {
                Vector2 start = transform.position;
                Vector2 end = start + new Vector2(horizontal, vertical);
                base.boxCollider.enabled = false;
                RaycastHit2D hit = Physics2D.Linecast(start, end, base.blockingLayer);
                base.boxCollider.enabled = true;
                if (hit.transform != null)
                {
                    switch (hit.transform.gameObject.tag)
                    {
                        case "Wall":
                            canMove = AttemptMove<Wall>(horizontal, vertical);
                            break;
                        case "Chest":
                            canMove = AttemptMove<Chest>(horizontal, vertical);
                            break;
                        case "Enemy":
                            canMove = AttemptMove<Enemy>(horizontal, vertical);
                            break;
                    }
                }
                else
                {
                    canMove = AttemptMove<Wall>(horizontal, vertical);
                }

                if (canMove && onWorldBoard)
                {
                    position.x += horizontal;
                    position.y += vertical;
                    GameManager.instance.updateBoard(horizontal, vertical);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Exit")
        {
            dungeonTransition = true;
            Invoke("GoDungeonPortal", 0.5f);
            Destroy(collision.gameObject);
        }
        else if(collision.tag == "Food" || collision.tag == "Soda")
        {
            UpdateHealth(collision);
            Destroy(collision.gameObject);
        }
        else if(collision.tag == "Item")
        {
            UpdateInventory(collision);
            Destroy(collision.gameObject);
            AdaptDifficulty();
        }
        else if(collision.tag == "Weapon")
        {
            if(weapon)
            {
                Destroy(transform.GetChild(0).gameObject);
            }
            collision.enabled = false;
            collision.transform.parent = transform;
            weapon = collision.GetComponent<Weapon>();
            weapon.AcquireWeapon();
            weapon.inPlayerInventory = true;
            weapon.enableSpriteRenderer(false);
            wallDamage = attackMod + 3;
            weaponComp1.sprite = weapon.getComponentImage(0);
            weaponComp2.sprite = weapon.getComponentImage(1);
            weaponComp3.sprite = weapon.getComponentImage(2);
            AdaptDifficulty();
        }
    }

    //AttemptMove overrides the AttemptMove function in the base class MovingObject
    //AttemptMove takes a generic parameter T which for Player will be of the type Wall, it also takes integers for x and y direction to move in.
    protected override bool AttemptMove<T>(int xDir, int yDir)
    {
        if(xDir == 1 && !isFacingRight)
        {
            isFacingRight = true;
        }
        else if (xDir == -1 && isFacingRight)
        {
            isFacingRight = false;
        }
        //Call the AttemptMove method of the base class, passing in the component T (in this case Wall) and x and y direction to move.
        bool hit = base.AttemptMove<T>(xDir, yDir);

        //Set the playersTurn boolean of GameManager to false now that players turn is over.
        GameManager.instance.playersTurn = false;

        return hit;
    }


    //OnCantMove overrides the abstract function OnCantMove in MovingObject.
    //It takes a generic parameter T which in the case of Player is a Wall which the player can attack and destroy.
    protected override void OnCantMove<T>(T component)
    {
        if(typeof(T) == typeof(Wall))
        {
            Wall hitWall = component as Wall;
            hitWall.DamageWall(wallDamage);
        }
        else if(typeof(T) == typeof(Chest))
        {
            Chest hitChest = component as Chest;
            hitChest.Open();
        }
        else if(typeof(T) == typeof(Enemy))
        {
            Enemy enemy = component as Enemy;
            enemy.DamageEnemy(wallDamage);
        }
        if(weapon)
        {
            weapon.useWeapon();
        }
        animator.SetTrigger("playerChop");
    }

    //LoseHealth is called when an enemy attacks the player.
    //It takes a parameter loss which specifies how many points to lose.
    public void LoseHealth(int loss)
    {
        //Set the trigger for the player animator to transition to the playerHit animation.
        animator.SetTrigger("playerHit");

        //Subtract lost health points from the players total.
        health -= loss;

        //Update the health display with the new total.
        healthText.text = "-" + loss + " Health: " + health;

        //Check to see if game has ended.
        CheckIfGameOver();
    }


    //CheckIfGameOver checks if the player is out of health points and if so, ends the game.
    private void CheckIfGameOver()
    {
        //Check if health point total is less than or equal to zero.
        if (health <= 0)
        {
            //Call the GameOver function of GameManager.
            GameManager.instance.GameOver();
        }
    }

    private void GoDungeonPortal()
    {
        if(onWorldBoard)
        {
            onWorldBoard = false;
            GameManager.instance.enterDungeon();
            transform.position = DungeonManager.startPos;
        }
        else
        {
            onWorldBoard = true;
            GameManager.instance.exitDungeon();
            transform.position = position;
        }
    }
    private void UpdateHealth(Collider2D item)
    {
        if (health < 100)
        {
            if (item.tag == "Food")
            {
                health += Random.Range(1, 4);
            }
            else
            {
                health += Random.Range(4, 11);
            }
            GameManager.instance.healthPoints = health;
            healthText.text = "Health: " + health;
        }
    }

    private void UpdateInventory(Collider2D item)
    {
        Item itemData = item.GetComponent<Item>();
        switch(itemData.type)
        {
            case itemType.glove:
                if(!inventory.ContainsKey("glove"))
                {
                    inventory.Add("glove", itemData);
                }
                else
                {
                    inventory["glove"] = itemData;
                }
                gloves.color = itemData.level;
                break;
            case itemType.boot:
                if(!inventory.ContainsKey("boot"))
                {
                    inventory.Add("boot", itemData);
                }
                else
                {
                    inventory["boot"] = itemData;
                }
                boot.color = itemData.level;
                break;
        }

        attackMod = 0;
        defenseMod = 0;

        foreach(var gear in inventory)
        {
            attackMod += gear.Value.attackMod;
            defenseMod += gear.Value.defenseMod;
        }

        if(weapon)
        {
            wallDamage = attackMod + 3;
        }
    }

    private void AdaptDifficulty()
    {
        if (wallDamage >= 10)
            GameManager.instance.enemiesSmarter = true;
        if (wallDamage >= 15)
            GameManager.instance.enemiesFaster = true;
        if (wallDamage >= 20)
            GameManager.instance.enemySpawnRatio = 10;
    }
}

