using UnityEngine;
using System.Collections;

public class Wall : MonoBehaviour
{
    public Sprite dmgSprite;
    public int hp = 3;
    public GameObject[] foodTiles;

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        //Get a component reference to the SpriteRenderer.
        spriteRenderer = GetComponent<SpriteRenderer>();
    }


    //DamageWall is called when the player attacks a wall.
    public void DamageWall(int loss)
    {
        spriteRenderer.sprite = dmgSprite;
        hp -= loss;

        if (hp <= 0)
        {
            if(Random.Range(0, 5) == 1)
            {
                GameObject toInstantiate = foodTiles[Random.Range(0, foodTiles.Length)];
                GameObject instance = Instantiate(toInstantiate, transform.position, Quaternion.identity) as GameObject;
                instance.transform.SetParent(transform.parent);
            }

            gameObject.SetActive(false);
        }
    }
}
