﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienBullet : MonoBehaviour {

    public Vector2 direction;
    public float speed = 1;
    private SpriteRenderer sRnderer;

    Utilities utilities;

    private void Start()
    {
        utilities = Utilities.instance;

        sRnderer = GetComponent<SpriteRenderer>();
    }

    void Update () {
        if (utilities.gameState == Utilities.GameState.STOPPED) return;

        transform.Translate(direction * speed);
        if (!sRnderer.isVisible)
        {
            Destroy(gameObject);
        }
	}
}
