﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigLaser : MonoBehaviour
{
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
            Debug.Log("WTF");
            Human human = collision.GetComponent<Human>();
            if (human.curState != Human.State.GROUNDED)
            {
                human.DamageSelf(100, transform.position);
            }
        }
    }
}
