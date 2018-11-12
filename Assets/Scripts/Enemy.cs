using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameObject explosion;
    public ParticleSystem hit;
    public float maxHealth;
    public float bounceBackAmount = 0.4f;
    public float rotateAmount = 2f;
    public float rotateTime = 0.2f;
    protected float health;
    protected Vector2 direction;
    protected Vector2 newDirection;
    protected AudioSource[] audioSource;
    protected SpriteRenderer spriteRenderer;
    protected CircleCollider2D circleCollider;

    float timeToBounceBack = 0.1f;

    protected virtual void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        circleCollider = GetComponent<CircleCollider2D>();
        audioSource = GetComponents<AudioSource>();
        health = maxHealth;
    }

    public void DamageEnemy(float damage, Vector2 hitPosition)
    {
        Vector2 directionToEnemy = ((Vector2)transform.position - hitPosition).normalized;
        health -= damage;

        if (health <= 0)
        {
            StartCoroutine(DestroySelf());
        }
        else
        {
            direction += Vector2.right * directionToEnemy.x * bounceBackAmount;
            Instantiate(hit, hitPosition, transform.rotation);
            int index = Random.Range(0, 5);
            audioSource[index].Play();

            float directionToHitY = directionToEnemy.x > 0 ? Mathf.Sign(directionToEnemy.y) : -Mathf.Sign(directionToEnemy.y);

            StartCoroutine(Rotate(directionToHitY));
        }
    }

    public virtual void DisinfectEnemy(Vector2 hitPoint)
    {
        Vector2 directionToEnemy = ((Vector2)transform.position - hitPoint).normalized;
        float directionToHitY = directionToEnemy.x > 0 ? Mathf.Sign(directionToEnemy.y) : -Mathf.Sign(directionToEnemy.y);

        int index = Random.Range(0, 5);
        audioSource[index].Play();
        StartCoroutine(Rotate(directionToHitY));
    }

    protected virtual IEnumerator DestroySelf()
    {
        spriteRenderer.color = new Color(0, 0, 0, 0);
        circleCollider.enabled = false;
        Instantiate(explosion, transform.position, transform.rotation);

        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
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
