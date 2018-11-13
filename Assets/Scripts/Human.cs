﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : Hittable
{
    public enum State { GROUNDED, ABDUCTED, FALLING, INFECTED, RESCUED, DEAD }
    public State curState;

    public float acceleration = 0.01f;

    private Transform currentGround;
    private float actualSpeed = 0;
    private GameObject player;
    private GameObject rightSide;
    private GameObject leftSide;
    private float verticalHalfSize;

    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider2D;

    void Start()
    {
        curState = State.GROUNDED;
        verticalHalfSize = Camera.main.orthographicSize;
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider2D = GetComponent<BoxCollider2D>();
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
                Destroy(gameObject);
            }
        }
        if(curState == State.RESCUED)
        {
            if (transform.position.y <= -verticalHalfSize + 0.8f)
            {
                transform.parent = currentGround;
                curState = State.GROUNDED;
            }
        }
        if(curState == State.GROUNDED)
        {
            transform.parent = currentGround;
        }
    }

    public override void DamageSelf(float damage, Vector2 hitPosition)
    {
        StartCoroutine(DestroySelf());
    }

    protected virtual IEnumerator DestroySelf()
    {
        spriteRenderer.color = new Color(0, 0, 0, 0);
        boxCollider2D.enabled = false;
        curState = State.DEAD;
        //Instantiate(explosion, transform.position, transform.rotation);

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
