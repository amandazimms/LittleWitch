using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RAPositionChange : MonoBehaviour
{
    public Vector3[] positions;

    Transform myTransform;

    void Awake()
    {
        myTransform = transform;
    }
}
