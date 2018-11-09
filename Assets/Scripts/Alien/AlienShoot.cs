using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienShoot : MonoBehaviour {

    private Vector2 direction;
    private float distance;
    private GameObject player;
    public GameObject bullet;

	// Use this for initialization
	void Start () {
		player = GameObject.FindWithTag("Player");
    }
	
	// Update is called once per frame
	void Update () {
        direction = new Vector2(player.transform.position.x - transform.position.x, player.transform.position.y - transform.position.y);
        distance = direction.magnitude;
        if (distance < 10)
        {
            //StartCoroutine(alienShoot());
        }
    }

    /*IEnumerator alienShoot()
    {
        direction.Normalize();
        GameObject alienBullet = Instantiate(bullet, transform.position + new Vector3(direction.x * .3f, direction.y * .3f, 0), transform.rotation);
    }*/
}
