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

    void Awake()
    {
        alien = GetComponent<Alien>();
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
        while (true && (alien.curState == Alien.State.PATROLLING || alien.curState == Alien.State.INFECTED) && playerTransform != null)
        {
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            GameObject alienBullet = Instantiate(bullet, transform.position, transform.rotation);
            AlienBullet tempBullet = alienBullet.GetComponent<AlienBullet>();
            tempBullet.direction = direction;
            float waitSeconds = 1.5f;
            if(alien.curState == Alien.State.INFECTED)
            {
                waitSeconds = 0.8f;
            }
            yield return new WaitForSeconds(waitSeconds);
        }
    }
}
