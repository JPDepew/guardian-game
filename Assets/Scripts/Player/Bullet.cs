using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Transform rayPos;
    public float speed = 1f;
    public float invisibleTime = 0.5f;
    public LayerMask layerMask;

    protected RaycastHit2D hit;
    SpriteRenderer spriteRenderer;
    private float direction;
    private bool shouldRaycast = true;

    private void Start()
    {
        direction = Mathf.Sign(transform.localScale.x);
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected void Update()
    {
        transform.Translate(Vector3.right * speed * direction);

        if (shouldRaycast)
        {
            Raycasting();
        }
        if (!spriteRenderer.isVisible)
        {
            StartCoroutine(DelayedDestroy());
        }
    }

    void Raycasting()
    {
        hit = Physics2D.Raycast(rayPos.position, Vector2.right * direction, speed, layerMask);
        Debug.DrawRay(rayPos.position, Vector2.right * direction * speed, Color.red);

        if (hit)
        {
            Transform hitObject = hit.transform;
            HitAction(hitObject, hit.point);
            StartCoroutine(DestroyObject());
        }
    }

    protected virtual void HitAction(Transform enemy, Vector2 hitPoint)
    {

    }

    protected IEnumerator DestroyObject()
    {
        shouldRaycast = false;
        speed = 0;
        
        while (spriteRenderer.color.a >= 0)
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, spriteRenderer.color.a - 0.1f);
            yield return null;
        }
        Destroy(gameObject);
    }

    IEnumerator DelayedDestroy()
    {
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }
}
