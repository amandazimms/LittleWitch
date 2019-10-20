using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathToVillage : MonoBehaviour
{
    public static Transform[] pointsTowardVillage;


    private void Awake()
    {
        pointsTowardVillage = new Transform[transform.childCount];

        for (int i = 0; i < pointsTowardVillage.Length; i++)
        {
            pointsTowardVillage[i] = transform.GetChild(i);
        }
    }
}
