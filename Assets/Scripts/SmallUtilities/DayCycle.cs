﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayCycle : MonoBehaviour
{
    [HideInInspector] public float sunriseSunsetLength = 5f;
    Animator anim;

    [HideInInspector] public bool hasStartedAnimFinished;
    [HideInInspector] public bool hasStartedAnimReachedKeyMoment;

    private void Awake()
    {
        anim = GetComponent<Animator>();

        anim.speed = 1 / sunriseSunsetLength;
    }

    public void Dawn()
    {
        StartCoroutine("Sunrise");
    }

    IEnumerator Sunrise()
    {
        anim.SetTrigger("Dawn");

        hasStartedAnimFinished = false;
        while (!hasStartedAnimFinished) { yield return null; }
    }

    public void Dusk()
    {
        StartCoroutine("Sunset");
    }

    IEnumerator Sunset()
    {
        anim.speed = 1 / sunriseSunsetLength;
        anim.SetTrigger("Dusk");

        hasStartedAnimFinished = false;
        while (!hasStartedAnimFinished) { yield return null; }
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