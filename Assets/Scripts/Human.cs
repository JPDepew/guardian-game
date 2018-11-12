using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : MonoBehaviour
{
    public enum State { GROUNDED, ABDUCTED, FALLING, INFECTED, RESCUED }
    public State curState;

    public float acceleration = 0.01f;

    private Transform currentGround;
    private float actualSpeed = 0;
    private GameObject player;
    private GameObject rightSide;
    private GameObject leftSide;
    private float verticalHalfSize;

    // Use this for initialization
    void Start()
    {
        curState = State.GROUNDED;
        player = GameObject.FindWithTag("Player");
        rightSide = GameObject.FindWithTag("rightMapSide");
        leftSide = GameObject.FindWithTag("leftMapSide");
        verticalHalfSize = Camera.main.orthographicSize;
    }

    // Update is called once per frame
    void Update()
    {
        if (curState == State.FALLING)
        {
            transform.parent = currentGround;
            if (transform.position.y > -verticalHalfSize + 1)
            {
                actualSpeed += acceleration;
                transform.Translate(Vector2.down * actualSpeed, Space.World);
            }
            else
            {
                curState = State.GROUNDED;
            }
        }
        if(curState == State.GROUNDED)
        {
            transform.parent = currentGround;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("rightMapSide") || collision.CompareTag("leftMapSide"))
        {
            currentGround = collision.transform;
        }
    }
}
