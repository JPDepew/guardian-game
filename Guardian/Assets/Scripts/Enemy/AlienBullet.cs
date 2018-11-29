using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienBullet : MonoBehaviour {

    public Vector2 direction;
    public float speed = 1;
    private SpriteRenderer sRnderer;
    private void Start()
    {
        sRnderer = GetComponent<SpriteRenderer>();
    }

    void Update () {
        transform.Translate(direction * speed);
        if (!sRnderer.isVisible)
        {
            Destroy(gameObject);
        }
	}
}
