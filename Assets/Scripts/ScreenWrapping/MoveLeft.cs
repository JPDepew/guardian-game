using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveLeft : MonoBehaviour
{
    private GameObject player;
    private GameObject rightSide;
    private GameObject leftSide;
    public float distance;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        rightSide = GameObject.FindWithTag("rightMapSide");
        leftSide = GameObject.FindWithTag("leftMapSide");
    }

    void Update()
    {
        if (player != null)
        {
            if (player.transform.position.x > rightSide.transform.position.x + 50)
            {
                Vector3 newPosition = new Vector3(rightSide.transform.position.x + distance, leftSide.transform.position.y, 0);
                leftSide.transform.position = newPosition;
            }
            else if (player.transform.position.x < rightSide.transform.position.x + 50)
            {
                Vector3 newPosition = new Vector3(rightSide.transform.position.x - distance, leftSide.transform.position.y, 0);
                leftSide.transform.position = newPosition;
            }
            if (player.transform.position.x > leftSide.transform.position.x + 50)
            {
                Vector3 newPosition = new Vector3(leftSide.transform.position.x + distance, rightSide.transform.position.y, 0);
                rightSide.transform.position = newPosition;
            }
            else if (player.transform.position.x < leftSide.transform.position.x + 50)
            {
                Vector3 newPosition = new Vector3(leftSide.transform.position.x - distance, rightSide.transform.position.y, 0);
                rightSide.transform.position = newPosition;
            }
        }
    }
}
