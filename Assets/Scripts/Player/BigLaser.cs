using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigLaser : Bullet {

    protected override void Update()
    {
        Raycasting();
    }

    protected override void HitAction(Transform enemy, Vector2 hitPoint)
    {
        enemy.GetComponent<Hittable>().DamageSelf(100, hitPoint);
    }
}