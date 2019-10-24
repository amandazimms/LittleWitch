using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticData : MonoBehaviour
{
    private static float playerStrength = .9f;

    public static float PlayerStrength
    {
        get { return playerStrength; }
        set {  playerStrength = value; }
    }
}
