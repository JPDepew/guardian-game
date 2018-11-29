using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDestroy : Bullet {

    protected override void HitAction(Transform enemy, Vector2 hitPoint)
    {
        enemy.GetComponent<Hittable>().DamageSelf(1, hitPoint);
    }
}
