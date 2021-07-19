using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateThis : MonoBehaviour
{
    public float rotateSpeed = 0.1f;
    public Vector3 rotateDir;

    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(rotateDir * rotateSpeed);
    }
}
