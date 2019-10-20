using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathToHut : MonoBehaviour
{
    public static Transform[] pointsTowardHut;

    private void Awake()
    {
        pointsTowardHut = new Transform[transform.childCount];

        for (int i = 0; i < pointsTowardHut.Length; i++)
        {
            pointsTowardHut[i] = transform.GetChild(i); 
        }
    }
}
