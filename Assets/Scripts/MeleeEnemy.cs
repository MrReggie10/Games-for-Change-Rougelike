using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : MonoBehaviour, IEnemy
{
    private Rigidbody2D rb;
    [SerializeField] private Transform attackHitboxCenter;
    [SerializeField] private GameObject meleeHitbox;

    [SerializeField] private int health;

    [SerializeField] private float speed;

    [SerializeField] private int damage;
    [SerializeField] private float attackDistance;
    [SerializeField] private float attackWindupTime;
    [SerializeField] private float attackTime;
    private bool attacking;

    public IEnemy.entityState entityState = IEnemy.entityState.freeMove;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if(entityState == IEnemy.entityState.freeMove)
        {
            if(Vector2.Distance(transform.position, PlayerSingleton.player.transform.position) < attackDistance && !attacking)
            {
                StartCoroutine(AttackCycle());
            }

            if (attacking)
            {
                rb.velocity = Vector2.zero;
            }
            else
            {
                rb.velocity = (PlayerSingleton.player.transform.position - transform.position).normalized * speed;
            }
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
        rb.velocity = Vector2.zero;
        entityState = IEnemy.entityState.stun;

        yield return new WaitForSeconds(secondsActive);

        entityState = IEnemy.entityState.freeMove;
    }

    private void Die()
    {
        //drop loot

        Destroy(gameObject);
    }

    private IEnumerator AttackCycle()
    {
        attacking = true;
        Vector3 playerPos = PlayerSingleton.player.transform.position;
        playerPos.z = 5.23f;

        Vector3 objectPos = transform.position;
        playerPos.x = playerPos.x - objectPos.x;
        playerPos.y = playerPos.y - objectPos.y;

        float angle = Mathf.Atan2(playerPos.y, playerPos.x) * Mathf.Rad2Deg;
        attackHitboxCenter.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        yield return new WaitForSeconds(attackWindupTime);

        meleeHitbox.SetActive(true);
        List<Collider2D> overlapColliders = new List<Collider2D>();
        meleeHitbox.GetComponent<CapsuleCollider2D>().OverlapCollider(new ContactFilter2D().NoFilter(), overlapColliders);

        foreach (Collider2D collider in overlapColliders)
        {
            if (collider.GetComponent<Health>() != null)
            {
                collider.GetComponent<Health>().Damage(damage);
            }
        }

        yield return new WaitForSeconds(attackTime);

        attacking = false;
        meleeHitbox.SetActive(false);
    }
}
