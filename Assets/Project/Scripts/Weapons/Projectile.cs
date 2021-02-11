using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    /*
     * projectile layer is set to bullet, it ignores itself
     */


    [Header("stats")]
    public float velocity = 400;
    public float lifeTime = 5f;
    public float inaccuracyFactor = 0.01f;
    public float damage = 10;
    [Space]

    [Header("Effects")]
    public GameObject hitEffect;
    public GameObject hitEffectEntity;
    

    private Rigidbody Rigidbody;    

    void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
    }
    void Start()
    {
        Destroy(this.gameObject, lifeTime);
        Rigidbody.velocity = (transform.forward + transform.right * Random.Range(-inaccuracyFactor, inaccuracyFactor) + transform.up * Random.Range(-inaccuracyFactor, inaccuracyFactor)) * velocity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Entity hitEntity = collision.gameObject.GetComponent<Entity>();


        if (!hitEntity)
        {
            if (hitEffect)
            {
                Instantiate(hitEffect, transform.position, Quaternion.LookRotation(-transform.forward, transform.up));
            }
        }
        else if (hitEntity)
        {
            if (hitEffectEntity)
            {
                Instantiate(hitEffectEntity, transform.position, Quaternion.LookRotation(-transform.forward, transform.up));
            }
            hitEntity.TakeDamage(damage);
        }

        Destroy(this.gameObject);
    }
}
