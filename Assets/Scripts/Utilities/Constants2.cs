using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants2 : MonoBehaviour
{
    public float wrapDst = 80f;
    public static Constants2 instance;
    private void Awake()
    {
        instance = this;
    }
}
