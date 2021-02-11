using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    /*
     * An entity is anything that can explode/die/disable and has health.
     */

    [Header("Stats")]
    public float health = 100;

    [Header("On Death")]
    public bool destroy = false;
    [Space]
    public GameObject deathEffect;
    public GameObject deathEffect2;


    private bool dead = false;

    void Start()
    {
        Debug.Log(name + " : Alive!");   
    }
    public void TakeDamage(float damage)
    {
        if (dead)
        {
            return;
        }

        health -= damage;

        if(health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        dead = true;

        Debug.Log(name + " : Died!");

        if (deathEffect)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }
        if (deathEffect2)
        {
            Instantiate(deathEffect2, transform.position, Quaternion.identity);
        }
        
        if (destroy)
        {
            Destroy(this.gameObject);
        }
        
    }
}
