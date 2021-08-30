using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayCycle : MonoBehaviour
{
    DayInfo dayInfo;
    float sunriseSunsetLength;
    Animator anim;

    [HideInInspector] public bool hasStartedAnimFinished;
    [HideInInspector] public bool hasStartedAnimReachedKeyMoment;

    private void Awake(){
        anim = GetComponent<Animator>();
        dayInfo = gameManager.GetComponent<DayInfo>();

        dayInfo.OnDay.AddListener(Day);
        dayInfo.OnDusk.AddListener(Dusk);
        dayInfo.OnNight.AddListener(Night);
        dayInfo.OnDawn.AddListener(Dawn);


        anim.speed = 1 / sunriseSunsetLength;
    }

    private void Start() {
        sunriseSunsetLength = dayInfo.sunriseSunsetLength;
    }

    public void Dusk()  {
        StartCoroutine("Sunset");
    }

    IEnumerator Sunset() {
        anim.speed = 1 / sunriseSunsetLength;
        anim.SetTrigger("Dusk");

        hasStartedAnimFinished = false;
        while (!hasStartedAnimFinished) { yield return null; }
    }

    public void Night(){

    }

    public void Dawn() {
        StartCoroutine("Sunrise");
    }

    IEnumerator Sunrise() {
        anim.speed = 1 / sunriseSunsetLength;
        anim.SetTrigger("Dawn");

        hasStartedAnimFinished = false;
        while (!hasStartedAnimFinished) { yield return null; }
    }

    public void Day() {

    }

    public void CurrentAnimationFinished() {
        hasStartedAnimFinished = true;
    }
    public void CurrentAnimationKeyMoment() {
        hasStartedAnimReachedKeyMoment = true;
    }
}
