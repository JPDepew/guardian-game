using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigLaser : MonoBehaviour
{

    //protected override void Update()
    //{
    //    Raycasting();
    //}

    //protected override void HitAction(Transform enemy, Vector2 hitPoint)
    //{
    //    enemy.GetComponent<Hittable>().DamageSelf(100, hitPoint);
    //}

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
        if (collision.tag == "Human")
        {
            Human human = collision.GetComponent<Human>();
            if (human.curState != Human.State.GROUNDED)
            {
                human.DamageSelf(100, transform.position);
            }
        }
        //if(collision.tag == "MutatedAlien")
        //{
        //    MutatedAlien alien = collision.GetComponent<MutatedAlien>();
        //    if (alien.GetComponent<Renderer>().isVisible)
        //    {
        //        alien.DamageSelf(100, transform.position);
        //    }
        //}
    }
}
