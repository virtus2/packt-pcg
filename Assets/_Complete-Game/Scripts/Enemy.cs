using UnityEngine;
using System.Collections;

public class Enemy : MovingObject
{
    public int playerDamage;
    public int hp = 20;

    private Animator animator;
    private Transform target;
    private bool skipMove;
    private SpriteRenderer spriteRenderer;

    protected override void Start()
    {
        GameManager.instance.AddEnemyToList(this);

        animator = GetComponent<Animator>();

        target = GameObject.FindGameObjectWithTag("Player").transform;

        spriteRenderer = GetComponent<SpriteRenderer>();

        base.Start();
    }

    protected override bool AttemptMove<T>(int xDir, int yDir)
    {
        if (skipMove)
        {
            skipMove = false;
            return false;
        }

        base.AttemptMove<T>(xDir, yDir);
        skipMove = true;
        return true;
    }

    public void MoveEnemy()
    {
        int xDir = 0;
        int yDir = 0;
        if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)
            yDir = target.position.y > transform.position.y ? 1 : -1;
        else
            xDir = target.position.x > transform.position.x ? 1 : -1;

        AttemptMove<Player>(xDir, yDir);
    }

    protected override void OnCantMove<T>(T component)
    {
        Player hitPlayer = component as Player;
        hitPlayer.LoseHealth(playerDamage);
        animator.SetTrigger("enemyAttack");
    }

    public SpriteRenderer getSpriteRenderer()
    {
        return spriteRenderer;
    }

    public void DamageEnemy(int loss)
    {
        hp -= loss;

        if (hp <= 0)
        {
            GameManager.instance.RemoveEnemyFromList(this);
            Destroy(gameObject);
        }
    }

}