using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float health = 50f;
    public float maxHealth = 50f;
    public bool canDie = true;
    public float damageAmount;

    public HealthBar healthBar;

    private void Start()
    {
        healthBar.UpdateHealthBar(maxHealth, health);
    }

    private void Update()
    {
        if (health <= 0f)
        {
            if (canDie)
                canDie = false;

            else
                Invoke("Die", 0.1f);
        }

        healthBar.UpdateHealthBar(maxHealth, health);
    }

    public void TakeDamage(float amount, float delay)
    {
        damageAmount = amount;
        Invoke("TakingDamage", delay);
    }

    public void TakingDamage()
    {
        health -= damageAmount;
    }

    IEnumerator Die()
    {
        
        Destroy(gameObject);
        if (!canDie)
        {
            canDie = true;
        }
        return null;
    }
}
