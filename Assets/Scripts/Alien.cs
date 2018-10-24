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
    Vector2 direction;
    float verticalHalfSize;
    bool avoidingWall;


    // Use this for initialization
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        direction = Random.insideUnitCircle.normalized;
        verticalHalfSize = Camera.main.orthographicSize;
        StartCoroutine(ChangeDirection());
        StartCoroutine(AvoidWalls());
    }

    private void Update()
    {
        verticalHalfSize = Camera.main.orthographicSize;
        transform.Translate(direction * speed * Time.deltaTime);
    }

    IEnumerator ChangeDirection()
    {
        float timer = 0;
        while (true)
        {
            Vector2 newDirection = Random.insideUnitCircle.normalized;

            if (timer > timeToChangeDirection)
            {
                while (!(direction.x <= newDirection.x + 0.01f && direction.x >= newDirection.x - 0.01f && direction.y >= newDirection.y - 0.01f && direction.y <= newDirection.y + 0.01f))
                {
                    direction = Vector2.Lerp(direction, newDirection, easeToNewDirection);
                    if (avoidingWall)
                    {
                        timer = 0;
                        break;
                    }
                    yield return null;
                }
                timer = 0;
            }
            timer += Time.deltaTime;
            yield return new WaitForSeconds(0.05f);
        }
    }

    IEnumerator AvoidWalls()
    {
        while (true)
        {
            Vector2 newDirection = Random.insideUnitCircle.normalized;

            if (transform.position.y > verticalHalfSize - 1)
            {
                avoidingWall = true;
                newDirection = new Vector2(newDirection.x, -Mathf.Abs(newDirection.y));
                while (!(direction.x <= newDirection.x + 0.01f && direction.x >= newDirection.x - 0.01f && direction.y >= newDirection.y - 0.01f && direction.y <= newDirection.y + 0.01f))
                {
                    direction = Vector2.Lerp(direction, newDirection, easeToNewDirection);
                    yield return null;
                }
                avoidingWall = false;
            }
            if (transform.position.y < -verticalHalfSize + 1)
            {
                avoidingWall = true;
                newDirection = new Vector2(newDirection.x, Mathf.Abs(newDirection.y));
                while (!(direction.x <= newDirection.x + 0.01f && direction.x >= newDirection.x - 0.01f && direction.y >= newDirection.y - 0.01f && direction.y <= newDirection.y + 0.01f))
                {
                    direction = Vector2.Lerp(direction, newDirection, easeToNewDirection);
                    yield return null;
                }
                avoidingWall = false;
            }
            yield return null;
        }
    }
}
