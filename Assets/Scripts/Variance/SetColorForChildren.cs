using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SetColorForChildren : MonoBehaviour
{
    public bool getRends;
    public SpriteRenderer[] mySpriteRends;

    public bool setColor;
    public Color color;

    public Color copyForBackupDoesNothing;

    void Update()
    {
        if (getRends)
        {
            mySpriteRends = GetComponentsInChildren<SpriteRenderer>();
            getRends = false;
        }

        if (setColor)
        {
            foreach (SpriteRenderer rend in mySpriteRends)
                rend.color = color;
            setColor = false;
        }
    }
}
