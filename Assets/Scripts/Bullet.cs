using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Transform rayPos;
    public float speed = 1f;
    public float timeTillDeath = 1f;
    public float invisibleTime = 0.5f;
    public LayerMask layerMask;

    private float targetTime = 0;
    private float direction;
    private bool shouldRaycast = true;

    private void Start()
    {
        direction = Mathf.Sign(transform.localScale.x);
        targetTime = Time.time + timeTillDeath;
    }

    void Update()
    {
        transform.Translate(Vector3.right * speed * direction);
        if (Time.time > targetTime)
        {
            Destroy(gameObject);
        }
        if (shouldRaycast)
        {
            Raycasting();
        }
    }

    void Raycasting()
    {
        RaycastHit2D hit = Physics2D.Raycast(rayPos.position, Vector2.right * direction, speed, layerMask);
        Debug.DrawRay(rayPos.position, Vector2.right * direction * speed, Color.red);
        if (hit)
        {
            Transform enemy = hit.transform;
            Vector2 directionToEnemy = ((Vector2)enemy.position - hit.point).normalized;
            enemy.GetComponent<Enemy>().DamageEnemy(1, directionToEnemy);
            StartCoroutine(DestroyObject());
        }
    }

    IEnumerator DestroyObject()
    {
        shouldRaycast = false;
        GetComponent<SpriteRenderer>().enabled = false;// = new Color(0,0,0,0);
        speed = 0;
        yield return new WaitForSeconds(invisibleTime);
        Destroy(gameObject);
    }
}
