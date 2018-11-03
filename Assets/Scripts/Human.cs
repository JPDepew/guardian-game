using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : MonoBehaviour
{
    public float acceleration = 0.01f;
    public bool abducted { get; set; }
    public bool grounded { get; set; }

    private float actualSpeed = 0;
    private GameObject player;
    private GameObject rightSide;
    private GameObject leftSide;
    private float verticalHalfSize;

    // Use this for initialization
    void Start()
    {
        abducted = false;
        grounded = true;
        player = GameObject.FindWithTag("Player");
        rightSide = GameObject.FindWithTag("rightMapSide");
        leftSide = GameObject.FindWithTag("leftMapSide");
        verticalHalfSize = Camera.main.orthographicSize;
    }

    // Update is called once per frame
    void Update()
    {
        if (!abducted)
        {
            if (transform.position.y > -verticalHalfSize + 1)
            {
                actualSpeed += acceleration;
                transform.Translate(Vector2.down * actualSpeed, Space.World);
            }
            else
            {
                grounded = true;
            }
        }
    }

    public void SetAbducted()
    {
        grounded = false;
        abducted = true;
    }
}
