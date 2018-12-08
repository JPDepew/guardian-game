using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Alien")
        {
            Enemy alien = collision.GetComponent<Enemy>();
            if (alien.GetComponent<Renderer>().isVisible)
            {
                alien.DamageSelf(100, transform.position);
            }
        }
        if (collision.tag == "AlienBullet")
        {
            Destroy(collision.gameObject);
        }
    }
}
