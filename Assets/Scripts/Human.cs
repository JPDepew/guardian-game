using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : MonoBehaviour {

    public bool abducted;

    private GameObject player;
    private GameObject rightSide;
    private GameObject leftSide;

    // Use this for initialization
    void Start () {
        player = GameObject.FindWithTag("Player");
        rightSide = GameObject.FindWithTag("rightMapSide");
        leftSide = GameObject.FindWithTag("leftMapSide");
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
