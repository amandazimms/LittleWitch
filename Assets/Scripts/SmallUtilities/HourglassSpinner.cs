using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HourglassSpinner : MonoBehaviour
{
    bool isRightSideUp;

    //public enum RotateStage { Rotate, Falling, Rotate2, Falling2, Idle }
    //public RotateStage currentRotateStage;

    [HideInInspector] public Animator anim;
    [HideInInspector] public bool hasStartedAnimFinished;
    [HideInInspector] public bool hasStartedAnimReachedKeyMoment;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public IEnumerator Rotate(float speedForFallingState)
    {
        if (isRightSideUp)
        {
            anim.SetTrigger("Rotate");

            hasStartedAnimFinished = false; 
            while (!hasStartedAnimFinished) { yield return null; }

            anim.SetTrigger("Falling");
            anim.speed = speedForFallingState;

            hasStartedAnimFinished = false;
            while (!hasStartedAnimFinished) { yield return null; }

            anim.speed = 1;
            isRightSideUp = false;
            yield break;
        }
        else if (!isRightSideUp)
        {
            anim.SetTrigger("Rotate2");

            hasStartedAnimFinished = false;
            while (!hasStartedAnimFinished) { yield return null; }

            anim.SetTrigger("Falling2");
            anim.speed = speedForFallingState;

            hasStartedAnimFinished = false;
            while (!hasStartedAnimFinished) { yield return null; }

            anim.speed = 1;
            isRightSideUp = true;
            yield break;
        }
    }

    public void CurrentAnimationFinished()
    {
        hasStartedAnimFinished = true;
    }
    public void CurrentAnimationKeyMoment()
    {
        hasStartedAnimReachedKeyMoment = true;
    }
}
