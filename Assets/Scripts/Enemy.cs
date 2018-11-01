using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public ParticleSystem hit;
    public float maxHealth;
    public float bounceBackAmount = 0.4f;
    public float rotateAmount = 2f;
    public float rotateTime = 0.2f;
    protected float health;
    protected Vector2 direction;
    protected Vector2 newDirection;

    float timeToBounceBack = 0.1f;

    protected virtual void Start()
    {
        health = maxHealth;
    }

    public void DamageEnemy(float damage, Vector2 hitPosition)
    {
        Vector2 directionToEnemy = ((Vector2)transform.position - hitPosition).normalized;
        health -= damage;
        direction += Vector2.right * directionToEnemy.x * bounceBackAmount;
        Instantiate(hit, hitPosition, transform.rotation);

        float directionToHitY = directionToEnemy.x > 0 ? Mathf.Sign(directionToEnemy.y) : -Mathf.Sign(directionToEnemy.y);

        StartCoroutine(Rotate(directionToHitY));
    }

    IEnumerator Rotate(float directionToHitY)
    {
        float timer = Time.time + rotateTime;
        while (Time.time < timer)
        {
            transform.Rotate(new Vector3(0, 0, directionToHitY * rotateAmount));
            yield return null;
        }
        timer = Time.time + rotateTime;
        while (Time.time < timer)
        {
            transform.Rotate(new Vector3(0, 0, -directionToHitY * rotateAmount));
            yield return null;
        }
    }
}
