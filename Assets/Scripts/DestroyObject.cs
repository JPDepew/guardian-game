using System.Collections;
using UnityEngine;

public class DestroyObject : MonoBehaviour
{
    public float destroyAfter = 1f;

    private void Start()
    {
        StartCoroutine(DestroyAfter());
    }

    IEnumerator DestroyAfter()
    {
        yield return new WaitForSeconds(destroyAfter);
        Destroy(gameObject);
    }
}