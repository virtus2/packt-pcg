using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public Sprite openSprite;
    public Item randomItem;
    public Weapon weapon;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Open()
    {
        spriteRenderer.sprite = openSprite;
        GameObject toInstantiate;
        if (Random.Range(0,2) == 1)
        {
            randomItem.RandomItemInit();
            toInstantiate = randomItem.gameObject;
        }
        else
        {
            toInstantiate = weapon.gameObject;
        }
        GameObject instance = Instantiate(toInstantiate, transform.position, Quaternion.identity) as GameObject;
        instance.transform.SetParent(transform.parent);
        gameObject.layer = 10;
        spriteRenderer.sortingLayerName = "Items";
    }
}
