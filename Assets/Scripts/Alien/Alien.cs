using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alien : Enemy
{
    public CircleCollider2D detector;
    public float speed;
    public float timeToChangeDirection = 3;
    public float easeToNewDirection = 0.3f;

    AudioSource audioSource;
    SpriteRenderer spriteRenderer;
    private bool hasHuman;
    float verticalHalfSize;
    bool avoidingWall;


    // Use this for initialization
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
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

    public void ChaseHuman(Transform human)
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

            //Vector2 newDirection = Random.insideUnitCircle.normalized;

            //if (timer > timeToChangeDirection)
            //{
            //    while (!(direction.x <= newDirection.x + 0.01f && direction.x >= newDirection.x - 0.01f && direction.y >= newDirection.y - 0.01f && direction.y <= newDirection.y + 0.01f))
            //    {
            //        direction = Vector2.Lerp(direction, newDirection, easeToNewDirection);
            //        if (avoidingWall)
            //        {
            //            timer = 0;
            //            break;
            //        }
            //        yield return null;
            //    }
            //    timer = 0;
            //}
            //timer += Time.deltaTime;
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
                //avoidingWall = true;
                //newDirection = new Vector2(newDirection.x, -Mathf.Abs(newDirection.y));
                //while (!(direction.x <= newDirection.x + 0.1f && direction.x >= newDirection.x - 0.1f && direction.y >= newDirection.y - 0.1f && direction.y <= newDirection.y + 0.1f))
                //{
                //    direction = Vector2.Lerp(direction, newDirection, easeToNewDirection);
                //    yield return null;
                //}
                //avoidingWall = false;
            }
            if (transform.position.y < -verticalHalfSize + 2)
            {
                newDirection = new Vector2(newDirection.x, Mathf.Abs(newDirection.y));
                //avoidingWall = true;
                //newDirection = new Vector2(newDirection.x, Mathf.Abs(newDirection.y));
                //while (!(direction.x <= newDirection.x + 0.1f && direction.x >= newDirection.x - 0.1f && direction.y >= newDirection.y - 0.1f && direction.y <= newDirection.y + 0.1f))
                //{
                //    direction = Vector2.Lerp(direction, newDirection, easeToNewDirection);
                //    yield return null;
                //}
                //avoidingWall = false;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator ChasingHuman(Transform human)
    {
        while (true)
        {
            newDirection = -(transform.position - human.position).normalized;
            yield return null;
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Human" && !hasHuman)
        {
            collision.transform.parent = transform;
            newDirection = Vector2.up;
            StopCoroutine("ChasingHuman");
            hasHuman = true;
        }
    }
}
