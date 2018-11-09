using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienBullet : MonoBehaviour {

    public Vector2 direction;
    public float speed = 1;

	void Update () {
        transform.Translate(direction * speed);
	}
}
