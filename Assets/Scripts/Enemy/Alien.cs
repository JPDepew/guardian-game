using System.Collections;
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

    public delegate void OnDestroyed();
    public static event OnDestroyed onAlienDestroyed;
    public GameObject infectedAlien;

    protected override void Start()
    {
        base.Start();

        curState = State.PATROLLING;
        direction = Vector2.zero;// = Random.insideUnitCircle.normalized;
        verticalHalfSize = Camera.main.orthographicSize;
        StartCoroutine("ChangeDirection");
        StartCoroutine("AvoidWalls");
        StartCoroutine(NullParentTimer());
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
        StartCoroutine("DelayedFindParent");
    }

    IEnumerator NullParentTimer()
    {
        while (true)
        {
            yield return new WaitForSeconds(3);
            if(transform.parent == null)
            {
                transform.parent = GameObject.FindGameObjectWithTag("leftMapSide").transform;
            }
        }
    }

    IEnumerator ChangeDirection()
    {
        curState = State.PATROLLING;
        while (true)
        {
            newDirection = Random.insideUnitCircle.normalized;
            yield return new WaitForSeconds(timeToChangeDirection);
        }
    }

    IEnumerator DelayedFindParent()
    {
        yield return new WaitForSeconds(2f);
        if(transform.parent == null)
        {
            Debug.Log("parent is null");
            transform.parent = GameObject.FindGameObjectWithTag("rightMapSide").transform;
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
            if (_human && (_human.curState == Human.State.GROUNDED || _human.curState == Human.State.FALLING))
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

    public override void DamageSelf(float damage, Vector2 hitPosition)
    {
        base.DamageSelf(damage, hitPosition);
        int index = Random.Range(0, 6);
        audioSources[index].Play();
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        if (collision.tag == "Human" && !human)
        {
            human = collision.GetComponent<Human>();

            if (human.curState == Human.State.FALLING || human.curState == Human.State.GROUNDED)
            {
                collision.transform.position = new Vector2(transform.position.x, transform.position.y - humanOffset);
                collision.transform.parent = transform;
                human.curState = Human.State.ABDUCTED;

                StopAllCoroutines();
                StartCoroutine("Abducting");
            }
            else
            {
                curState = State.PATROLLING;
                human = null;
            }
        }
    }

    // JOSIAH - LOOK HERE
    IEnumerator Abducting()
    {
        newDirection = Vector2.up;
        curState = State.ABDUCTING;
        bool infectedHuman = false;
        while (human && human.curState != Human.State.DEAD)
        {
            if (transform.position.y > verticalHalfSize - 1)
            {
                Instantiate(infectedAlien, new Vector2(transform.position.x, transform.position.y - 0.3f), Quaternion.Euler(Vector2.zero));
                Destroy(gameObject);
                break;
            }

            yield return null;
        }

        if (!infectedHuman)
        {
            human = null;
            // This occurs if he loses the human
            StartCoroutine("ChangeDirection");
            StartCoroutine("AvoidWalls");
        }
    }

    protected override void DestroySelf()
    {
        speed = 0;
        Destroy(windows);
        audioSources[6].Stop();

        PlayerStats.instance.IncreaseScoreBy(150);
        if (human)
        {
            human.SetToFalling(transform.parent);
        }
        if (onAlienDestroyed != null)
        {
            onAlienDestroyed();
        }
        scoreText = Instantiate(scoreText, new Vector3(transform.position.x, transform.position.y, -5), transform.rotation);
        scoreText.text = "150";
        base.DestroySelf();
    }
}
