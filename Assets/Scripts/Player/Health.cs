using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int totalHealth;
    public int health;

    // Start is called before the first frame update
    void Start()
    {
        health = totalHealth;
    }

    public void Damage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        //end screen

        Destroy(gameObject);
    }
}
