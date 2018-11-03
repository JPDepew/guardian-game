using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alien : Enemy
{
    public enum State { PATROLLING, CHASING, ABDUCTING, INFECTED }

    public State curState;

    public float speed;
    public float timeToChangeDirection = 3;
    public float easeToNewDirection = 0.3f;
    public float humanOffset = 0.8f;


    public GameObject windows;

    private Human human;
    private bool hasHuman;
    private bool isChasingHuman = false;
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
        verticalHalfSize = Camera.main.orthographicSize;

        direction = Vector2.Lerp(direction, newDirection, easeToNewDirection);
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
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
                newDirection = Vector2.up;
                StopAllCoroutines();
                curState = State.ABDUCTING;
                human.abducted = true;
                hasHuman = true;
            }
            else
            {
                curState = State.PATROLLING;
                human = null;
            }
        }
    }

    protected override IEnumerator DestroySelf()
    {
        Destroy(windows);
        int index = Random.Range(6, 9);
        audioSource[index].Play();
        if (human)
        {
            human.abducted = false;
            human.transform.SetParent(null);
        }
        return base.DestroySelf();
    }
}
