using UnityEngine;

public class DestroyObject : MonoBehaviour
{
    public float destroyAfter = 1f;

    private float destroyPoint;

    private void Start()
    {
        destroyPoint = Time.time + destroyAfter;
    }

    void Update()
    {
        if(Time.time > destroyPoint)
        {
            Destroy(gameObject);
        }
    }
}