using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDisinfect : Bullet {

    protected override void HitAction(Transform enemy, Vector2 hitPoint)
    {
        enemy.GetComponent<Enemy>().DisinfectEnemy(hitPoint);
    }
}
