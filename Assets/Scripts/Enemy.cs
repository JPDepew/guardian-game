using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    public float maxHealth;
    public float bounceBackAmount = 0.4f;
    public float rotateAmount = 2f;
    public float rotateTime = 0.2f;
    protected float health;
    protected Vector2 direction;

    float timeToBounceBack = 0.1f;

    private void Start()
    {
        health = maxHealth;
    }

    public void DamageEnemy(float damage, Vector2 directionToHit)
    {
        health -= damage;
        direction += Vector2.right * directionToHit.x * bounceBackAmount;
        float directionToHitY = directionToHit.x > 0 ? Mathf.Sign(directionToHit.y) : -Mathf.Sign(directionToHit.y);

        StartCoroutine(Rotate(directionToHitY));
    }

    IEnumerator Rotate(float directionToHitY)
    {
        float timer = Time.time + rotateTime;
        while(Time.time < timer)
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
