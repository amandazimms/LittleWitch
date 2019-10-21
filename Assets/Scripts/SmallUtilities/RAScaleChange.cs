using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RAScaleChange : MonoBehaviour
{
    public Vector3[] scales;
    
    public Transform myTransform;

    void Awake()
    {
        myTransform = transform;
    }
}
