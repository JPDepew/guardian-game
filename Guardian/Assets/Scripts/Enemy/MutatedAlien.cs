using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MutatedAlien : Enemy
{
    public float speed = 8;
    public float easeToNewDirection = 0.2f;
    public GameObject human;
    public GameObject disinfectExplosion;
    public float disinfectHumanOffset = 0.3f;
    public float destroyDelay = 0.4f;
    public float disinfectHealth = 3;
    public float dstToAttack = 3f;
    public GameObject hitMask;
    public GameObject disinfectMask;

    public delegate void OnDestroyed();
    public static event OnDestroyed onMutatedAlienDestroyed;

    float randomXOffset = 0;

    protected override void Start()
    {
        StartCoroutine(ChasePlayer());
        randomXOffset = Random.Range(-2, 2);
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        direction = Vector2.Lerp(direction, newDirection, easeToNewDirection);
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    public override void DamageSelf(float damage, Vector2 hitPosition)
    {
        int index = Random.Range(0, 2);
        audioSources[index].Play();
        GameObject tempMask = Instantiate(hitMask, hitPosition, transform.rotation);
        tempMask.transform.parent = transform;
        base.DamageSelf(damage, hitPosition);
    }

    public override void DisinfectEnemy(Vector2 hitPoint)
    {
        disinfectHealth--;
        GameObject tempMask = Instantiate(hitMask, hitPoint, transform.rotation);
        tempMask.transform.localScale *= 2;
        tempMask.transform.parent = transform;
        if (disinfectHealth <= 0)
        {
            if (onMutatedAlienDestroyed != null)
            {
                onMutatedAlienDestroyed();
            }
            speed = speed / 2;
            GameObject temp = Instantiate(disinfectMask, new Vector2(transform.position.x, transform.position.y + 0.3f), transform.rotation);
            temp.transform.parent = transform;
            StartCoroutine(DestroyAfterDelay());
        }
    }

    IEnumerator DestroyAfterDelay()
    {
        GetComponent<PolygonCollider2D>().enabled = false;
        yield return new WaitForSeconds(destroyDelay);
        GameObject newHuman = Instantiate(human, new Vector2(transform.position.x, transform.position.y - disinfectHumanOffset), Quaternion.Euler(Vector2.zero));
        newHuman.GetComponent<Human>().curState = Human.State.FALLING;
        Destroy(gameObject);
    }

    protected override void DestroySelf()
    {
        PlayerStats.instance.IncreaseScoreBy(50);
        if (onMutatedAlienDestroyed != null)
        {
            onMutatedAlienDestroyed();
        }
        base.DestroySelf();
    }

    IEnumerator ChasePlayer()
    {
        ShipController player = FindObjectOfType<ShipController>();
        float actualXOffset = randomXOffset;
        while (true)
        {
            if (player == null || player.shouldDestroyShip)
            {
                newDirection = Vector2.left;
                player = FindObjectOfType<ShipController>();
            }
            else
            {
                if((transform.position - player.transform.position).magnitude < dstToAttack)
                {
                    actualXOffset = 0;
                }
                else
                {
                    actualXOffset = randomXOffset;
                }
                newDirection = new Vector2(player.transform.position.x - transform.position.x, player.transform.position.y - transform.position.y + actualXOffset).normalized;
            }

            yield return new WaitForSeconds(0.25f);
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "leftMapSide" || collision.tag == "rightMapSide")
        {
            transform.parent = collision.transform;
        }
    }
}

