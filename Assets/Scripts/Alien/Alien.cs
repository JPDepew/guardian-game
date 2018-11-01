using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alien : Enemy
{
    public float speed;
    public float timeToChangeDirection = 3;
    public float easeToNewDirection = 0.3f;
    public float humanOffset = 0.8f;

    SpriteRenderer spriteRenderer;
    private bool hasHuman;
    float verticalHalfSize;
    bool avoidingWall;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();
        direction = Vector2.zero;// = Random.insideUnitCircle.normalized;
        verticalHalfSize = Camera.main.orthographicSize;
        StartCoroutine("ChangeDirection");
        StartCoroutine("AvoidWalls");
    }

    private void Update()
    {
        verticalHalfSize = Camera.main.orthographicSize;

        direction = Vector2.Lerp(direction, newDirection, easeToNewDirection);
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    public void ChaseHuman(Human human)
    {
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

            //Vector2 newDirection = Random.insideUnitCircle.normalized;
            if (transform.position.y > verticalHalfSize - 1)
            {
                newDirection = new Vector2(newDirection.x, -Mathf.Abs(newDirection.y));
            }
            if (transform.position.y < -verticalHalfSize + 2)
            {
                newDirection = new Vector2(newDirection.x, Mathf.Abs(newDirection.y));
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator ChasingHuman(Human human)
    {
        while (true)
        {
            if (!human.abducted)
            {
                newDirection = -(transform.position - human.transform.position).normalized;
                yield return null;
            }
            else
            {
                StartCoroutine("ChangeDirection");
                StartCoroutine("AvoidWalls");
                break;
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Human" && !hasHuman)
        {
            Human human = collision.GetComponent<Human>();

            if (!human.abducted)
            {
                collision.transform.position = new Vector2(transform.position.x, transform.position.y - humanOffset);
                collision.transform.parent = transform;
                newDirection = Vector2.up;
                StopCoroutine("ChasingHuman");
                human.abducted = true;
                hasHuman = true;
            }
        }
    }
}
