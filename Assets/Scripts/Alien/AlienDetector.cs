using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienDetector : MonoBehaviour
{
    Alien alien;

    bool hasFoundHuman = false;

    void Start()
    {
        alien = GetComponentInParent<Alien>();
    }

    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Human" && !hasFoundHuman)
        {
            Human human = collision.GetComponent<Human>();

            if (!human.abducted)
            {
                alien.ChaseHuman(human);
                hasFoundHuman = true;
            }
        }
    }
}
