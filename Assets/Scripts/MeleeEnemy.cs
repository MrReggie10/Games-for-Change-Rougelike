using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : MonoBehaviour, IEnemy
{
    private Rigidbody2D rb;

    [SerializeField] private int health;

    [SerializeField] private float speed;

    public IEnemy.entityState entityState = IEnemy.entityState.freeMove;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if(entityState == IEnemy.entityState.freeMove)
        {
            transform.position = Vector2.MoveTowards(transform.position, PlayerSingleton.player.transform.position, Time.deltaTime * speed);
        }
    }

    public void Damage(int damage, float knockbackTime, float knockbackForce)
    {
        health -= damage;
        if(health <= 0)
        {
            Die();
        }

        StartCoroutine(ActivateStun(knockbackTime));
        Vector2 direction = (transform.position - PlayerSingleton.player.transform.position).normalized;
        rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
    }

    public IEnumerator ActivateStun(float secondsActive)
    {
        entityState = IEnemy.entityState.stun;

        yield return new WaitForSeconds(secondsActive);

        entityState = IEnemy.entityState.freeMove;
        rb.velocity = Vector2.zero;
    }

    private void Die()
    {
        //drop loot

        Destroy(gameObject);
    }
}
