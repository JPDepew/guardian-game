using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldRotate : MonoBehaviour {

    public float rotationSpeed = 1f;
	void Update () {
        transform.Rotate(Vector3.back, rotationSpeed);
	}
}
