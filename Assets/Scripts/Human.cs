using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : Hittable
{
    public enum State { GROUNDED, ABDUCTED, FALLING, INFECTED, RESCUED, DEAD }
    public State curState;

    public float acceleration = 0.01f;
    public GameObject explosion;
    private Transform currentGround;
    private float actualSpeed = 0;
    private GameObject player;
    private GameObject rightSide;
    private GameObject leftSide;
    private float verticalHalfSize;

    AudioSource[] audioSources;

    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider2D;

    void Start()
    {
        curState = State.GROUNDED;
        verticalHalfSize = Camera.main.orthographicSize;
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        audioSources = GetComponents<AudioSource>();
    }

    void Update()
    {
        if (curState == State.FALLING)
        {
            transform.parent = currentGround;
            if (transform.position.y > -verticalHalfSize + 0.8f)
            {
                actualSpeed += acceleration;
                transform.Translate(Vector2.down * actualSpeed, Space.World);
            }
            else
            {
                StartCoroutine(DestroySelf());
            }
        }
        else
        {
            actualSpeed = 0;
        }
        if (curState == State.RESCUED)
        {
            if (transform.position.y <= -verticalHalfSize + 0.8f)
            {
                transform.parent.GetComponent<ShipController>().human = null;
                audioSources[1].Play();
                transform.parent = currentGround;
                curState = State.GROUNDED;
            }
        }
        if (curState == State.GROUNDED)
        {
            gameObject.layer = 0;
            transform.parent = currentGround;
        }
        else
        {
            gameObject.layer = 8;
        }
    }

    public override void DamageSelf(float damage, Vector2 hitPosition)
    {
        if (curState == State.FALLING || curState == State.ABDUCTED || curState == State.INFECTED)
        {
            StartCoroutine(DestroySelf());
        }
    }

    protected virtual IEnumerator DestroySelf()
    {
        spriteRenderer.color = new Color(0, 0, 0, 0);
        boxCollider2D.enabled = false;
        curState = State.DEAD;
        //Instantiate(explosion, transform.position, transform.rotation);
        audioSources[0].Play();
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("rightMapSide") || collision.CompareTag("leftMapSide"))
        {
            currentGround = collision.transform;
        }
    }
}
