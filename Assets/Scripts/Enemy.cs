using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    public float maxHealth;
    protected float health;

    private void Start()
    {
        health = maxHealth;
    }
}
