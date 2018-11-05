using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienDetector : MonoBehaviour
{
    Alien alien;

    void Start()
    {
        alien = GetComponentInParent<Alien>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Human" && alien.curState == Alien.State.PATROLLING)
        {
            Human human = collision.GetComponent<Human>();

            if (!human.abducted)
            {
                alien.ChaseHuman(human);
                alien.curState = Alien.State.CHASING;
            }
        }
    }
}
