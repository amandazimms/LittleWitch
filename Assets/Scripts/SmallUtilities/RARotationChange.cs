using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RARotationChange : MonoBehaviour
{
    public Vector3[] rotations;

    public Transform myTransform;

    void Awake()
    {
        myTransform = transform;
    }
}
