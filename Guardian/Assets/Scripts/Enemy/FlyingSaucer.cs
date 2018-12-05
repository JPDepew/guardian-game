using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingSaucer : Enemy
{
    public float speed;
    public float easeToNewDirection = 0.3f;
    public Vector2 horizontalBounds;

    public GameObject powerup;
    public GameObject bullet;
    ShipController player;

    float verticalHalfSize;
    bool avoidingWall;
    bool goToTopOfPlayer = true;
    bool shooting = false;

    public delegate void OnDestroyed();
    public static event OnDestroyed onAlienDestroyed;

    protected override void Start()
    {
        base.Start();

        StartCoroutine(FindPlayer());

        direction = Vector2.zero;
        verticalHalfSize = Camera.main.orthographicSize;
    }

    private void Update()
    {
        verticalHalfSize = Camera.main.orthographicSize;

        direction = Vector2.Lerp(direction, newDirection, easeToNewDirection);
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    IEnumerator FindPlayer()
    {
        while (player == null)
        {
            player = FindObjectOfType<ShipController>();
            yield return new WaitForSeconds(0.3f);
        }
        StartCoroutine(ChasePlayer());
    }

    public override void DamageSelf(float damage, Vector2 hitPosition)
    {
        base.DamageSelf(damage, hitPosition);
        int index = Random.Range(0, 5);
        audioSources[index].Play();
    }

    IEnumerator ChasePlayer()
    {
        StartCoroutine(ChangeDirectionTimer());
        while (true)
        {
            if (player == null)
            {
                newDirection = Vector2.left;
                yield return FindPlayer();
                //player = FindObjectOfType<ShipController>();
            }
            else
            {
                Vector2 dirToPlayer = player.transform.position - transform.position;
                float dstToPlayer = dirToPlayer.magnitude;
                if (dstToPlayer < 5)
                {
                    float placement = goToTopOfPlayer ? 2 : -2;
                    if (transform.position.y > verticalHalfSize - 1 && goToTopOfPlayer)
                    {
                        newDirection = dirToPlayer.normalized;
                    }
                    else if (transform.position.y < -verticalHalfSize + 1 && !goToTopOfPlayer)
                    {
                        newDirection = dirToPlayer.normalized;
                    }
                    else
                    {
                        newDirection = new Vector2(dirToPlayer.x, dirToPlayer.y + placement).normalized;
                    }
                }
                else
                {
                    newDirection = dirToPlayer.normalized;
                }
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator ChangeDirectionTimer()
    {
        while (true)
        {
            if (player == null)
            {
                newDirection = Vector2.left;
                yield return FindPlayer();
                //player = FindObjectOfType<ShipController>();
            }
            else
            {
                ShootAtPlayer();
                yield return new WaitForSeconds(3f);
                goToTopOfPlayer = !goToTopOfPlayer;
            }
        }
    }

    protected override void DestroySelf()
    {
        speed = 0;
        audioSources[6].Stop();
        int randomNum = Random.Range(0, 2);
        if (randomNum == 0 && !PlayerStats.instance.bigLaser)
        {
            Instantiate(powerup, transform.position, transform.rotation);
        }
        scoreText = Instantiate(scoreText, new Vector3(transform.position.x, transform.position.y, -5), transform.rotation);
        scoreText.text = "300";
        PlayerStats.instance.IncreaseScoreBy(300);
        base.DestroySelf();
    }

    void ShootAtPlayer()
    {
        Vector2 direction = (player.transform.position - transform.position).normalized;
        GameObject alienBullet = Instantiate(bullet, transform.position, transform.rotation);
        AlienBullet tempBullet = alienBullet.GetComponent<AlienBullet>();
        tempBullet.speed *= 1.5f;
        tempBullet.direction = direction;
    }
}
