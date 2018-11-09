using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienShoot : MonoBehaviour
{

    //private Vector2 direction;
    private float distance;
    private GameObject player;
    public GameObject bullet;
    private Alien alien;

    void Start()
    {
        alien = GetComponent<Alien>();
    }

    // Update is called once per frame
    void Update()
    {
        //direction = new Vector2(player.transform.position.x - transform.position.x, player.transform.position.y - transform.position.y);
        //distance = direction.magnitude;
        //if (distance < 10)
        //{
        //    StartCoroutine(alienShoot());
        //}
    }

    public void StartAlienShooting(Transform playerTransform)
    {
        StartCoroutine("AlienShooting", playerTransform);
    }

    public void StopAlienShooting()
    {
        StopCoroutine("AlienShooting");
    }

    IEnumerator AlienShooting(Transform playerTransform)
    {
        while (true && alien.curState == Alien.State.PATROLLING)
        {
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            GameObject alienBullet = Instantiate(bullet, transform.position/* + new Vector3(direction.x * .3f, direction.y * .3f, 0)*/, transform.rotation);
            AlienBullet tempBullet = alienBullet.GetComponent<AlienBullet>();
            tempBullet.direction = direction;
            yield return new WaitForSeconds(1.5f);
        }
    }
}
