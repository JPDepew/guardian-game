using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : Hittable
{
    public TextMesh scoreText;
    public GameObject explosion;
    public ParticleSystem hit;
    public float maxHealth;
    public float bounceBackAmount = 0.4f;
    public float rotateAmount = 2f;
    public float rotateTime = 0.2f;
    protected float health;
    protected Vector2 direction;
    protected Vector2 newDirection;
    protected AudioSource[] audioSources;
    protected SpriteRenderer spriteRenderer;
    protected CircleCollider2D circleCollider;

    protected virtual void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        circleCollider = GetComponent<CircleCollider2D>();
        audioSources = GetComponents<AudioSource>();
        health = maxHealth;
    }

    public override void DamageSelf(float damage, Vector2 hitPosition)
    {
        Vector2 directionToEnemy = ((Vector2)transform.position - hitPosition).normalized;
        health -= damage;
        if (health <= 0)
        {
            DestroySelf();
        }
        else
        {
            direction += Vector2.right * directionToEnemy.x * bounceBackAmount;
            Instantiate(hit, hitPosition, transform.rotation);

            float directionToHitY = directionToEnemy.x > 0 ? Mathf.Sign(directionToEnemy.y) : -Mathf.Sign(directionToEnemy.y);

            StartCoroutine(Rotate(directionToHitY));
        }
    }

    public virtual void DisinfectEnemy(Vector2 hitPoint)
    {
        Vector2 directionToEnemy = ((Vector2)transform.position - hitPoint).normalized;
        float directionToHitY = directionToEnemy.x > 0 ? Mathf.Sign(directionToEnemy.y) : -Mathf.Sign(directionToEnemy.y);

        // TODO: find a better disinfect sound
        //int index = Random.Range(0, 5);
        //audioSource[index].Play();
        //StartCoroutine(Rotate(directionToHitY));
    }

    protected virtual void DestroySelf()
    {
        Destroy(gameObject);
        Instantiate(explosion, transform.position, transform.rotation);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "leftMapSide" || collision.tag == "rightMapSide")
        {
            transform.parent = collision.transform;
        }
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
