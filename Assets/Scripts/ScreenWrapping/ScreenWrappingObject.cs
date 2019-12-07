using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenWrappingObject : MonoBehaviour
{
    Transform mainCam;
    float wrapDst = 100;

    protected virtual void Start()
    {
        mainCam = Camera.main.transform;
        wrapDst = Constants.S.wrapDst;
    }

    protected virtual void Update()
    {
        // The camera is too far to the right
        if (mainCam.position.x - transform.position.x > wrapDst)
        {
            transform.position = new Vector3(mainCam.position.x + wrapDst - 10, transform.position.y, transform.position.z);
        }
        // Cam is too far to left
        else if (mainCam.position.x - transform.position.x < -wrapDst)
        {
            transform.position = new Vector3(mainCam.position.x - wrapDst + 10, transform.position.y, transform.position.z);
        }
    }
}
