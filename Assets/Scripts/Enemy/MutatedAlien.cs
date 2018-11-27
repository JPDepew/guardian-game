using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MutatedAlien : Enemy {

    public float speed = 8;
    public float easeToNewDirection = 0.2f;

    protected override void Start()
    {
        StartCoroutine(ChasePlayer());
        //base.Start();
    }

    // Update is called once per frame
    void Update () {
        direction = Vector2.Lerp(direction, newDirection, easeToNewDirection);
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    public override void DamageSelf(float damage, Vector2 hitPosition)
    {
        base.DamageSelf(damage, hitPosition);
    }

    public override void DisinfectEnemy(Vector2 hitPoint)
    {
        // 1) Instantiate a human
        // 2) Destroy itself
        base.DisinfectEnemy(hitPoint);
    }

    protected override void DestroySelf()
    {
        PlayerStats.instance.IncreaseScoreBy(50);
        base.DestroySelf();
    }

    IEnumerator ChasePlayer()
    {
        ShipController player = FindObjectOfType<ShipController>();

        while (true)
        {
            if (player == null || player.shouldDestroyShip)
            {
                newDirection = Vector2.left;
                player = FindObjectOfType<ShipController>();
            }
            else
            {
                newDirection = (player.transform.position - transform.position).normalized;
            }

            yield return new WaitForSeconds(0.25f);
        }
    }
}
