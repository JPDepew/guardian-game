﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alien : Enemy
{
    public enum State { PATROLLING, CHASING, ABDUCTING, INFECTED, DEAD }

    public State curState;

    public float speed;
    public float infectedSpeed;
    public float timeToChangeDirection = 3;
    public float easeToNewDirection = 0.3f;
    public float humanOffset = 0.8f;
    public Vector2 horizontalBounds;

    public SpriteRenderer windows;

    private Human human;
    float verticalHalfSize;
    bool avoidingWall;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();

        curState = State.PATROLLING;
        direction = Vector2.zero;// = Random.insideUnitCircle.normalized;
        verticalHalfSize = Camera.main.orthographicSize;
        StartCoroutine("ChangeDirection");
        StartCoroutine("AvoidWalls");
    }

    private void Update()
    {
        float speedToUse = curState == State.INFECTED ? infectedSpeed : speed;

        verticalHalfSize = Camera.main.orthographicSize;

        direction = Vector2.Lerp(direction, newDirection, easeToNewDirection);
        transform.Translate(direction * speedToUse * Time.deltaTime, Space.World);
    }

    public void ChaseHuman(Human human)
    {
        curState = State.CHASING;
        StopCoroutine("ChangeDirection");
        StopCoroutine("AvoidWalls");
        StartCoroutine("ChasingHuman", human);
    }

    IEnumerator ChangeDirection()
    {
        while (true)
        {
            newDirection = Random.insideUnitCircle.normalized;
            yield return new WaitForSeconds(timeToChangeDirection);
        }
    }

    IEnumerator AvoidWalls()
    {
        while (true)
        {
            if (transform.position.y > verticalHalfSize - 1)
            {
                newDirection = new Vector2(newDirection.x, -Mathf.Abs(newDirection.y));
            }
            if (transform.position.y < -verticalHalfSize + 2)
            {
                newDirection = new Vector2(newDirection.x, Mathf.Abs(newDirection.y));
            }
            if (transform.localPosition.x > horizontalBounds.y)
            {
                newDirection = new Vector2(-Mathf.Abs(newDirection.x), newDirection.y);
            }
            if (transform.localPosition.x < horizontalBounds.x)
            {
                newDirection = new Vector2(Mathf.Abs(newDirection.x), newDirection.y);
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator ChasingHuman(Human _human)
    {
        while (true)
        {
            if (!_human.abducted)
            {
                newDirection = -(transform.position - _human.transform.position).normalized;
                yield return null;
            }
            else
            {
                StartCoroutine("ChangeDirection");
                StartCoroutine("AvoidWalls");
                curState = State.PATROLLING;
                break;
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Human" && !human)
        {
            human = collision.GetComponent<Human>();

            if (!human.abducted)
            {
                collision.transform.position = new Vector2(transform.position.x, transform.position.y - humanOffset);
                collision.transform.parent = transform;
                human.abducted = true;

                StopAllCoroutines();
                StartCoroutine("Abducting");
            }
            else
            {
                curState = State.PATROLLING;
                human = null;
            }
        }
        if (collision.tag == "leftMapSide" || collision.tag == "rightMapSide")
        {
            transform.parent = collision.transform;
        }
    }

    IEnumerator Abducting()
    {
        newDirection = Vector2.up;
        curState = State.ABDUCTING;
        while (true)
        {
            if (transform.position.y > verticalHalfSize - 1)
            {
                StartCoroutine("ChasePlayer");
                break;
            }

            yield return null;
        }
    }

    IEnumerator ChasePlayer()
    {
        ShipController player = FindObjectOfType<ShipController>();
        curState = State.INFECTED;
        StartCoroutine("FadeToRed", windows);
        StartCoroutine("FadeToRed", human.GetComponent<SpriteRenderer>());

        while (true)
        {
            if (player != null)
            {
                newDirection = (player.transform.position - transform.position).normalized;
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator FadeToRed(SpriteRenderer objectToFade)
    {
        if (objectToFade != null)
        {
            Color color = objectToFade.color;
            while (objectToFade.color.g > 0 && objectToFade != null)
            {
                objectToFade.color = new Color(objectToFade.color.r, objectToFade.color.g - 0.01f, objectToFade.color.b - 0.01f);
                yield return null;
            }
        }
    }

    protected override IEnumerator DestroySelf()
    {
        speed = 0;
        Destroy(windows);
        int index = Random.Range(6, 9);
        audioSource[index].Play();
        if (curState != State.INFECTED)
        {
            PlayerStats.instance.IncreaseScoreBy(150);
            if (human)
            {
                human.abducted = false;
                human.transform.SetParent(null);
            }
        }
        else // not infected
        {
            PlayerStats.instance.IncreaseScoreBy(50);
            if (human)
            {
                human.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
                human.GetComponent<BoxCollider2D>().enabled = false;
            }
        }
        curState = State.DEAD;
        return base.DestroySelf();
    }
}
