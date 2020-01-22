using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantGrow : MonoBehaviour
{
    public PlantPotStats plantPot;

    public enum GrowStage { Seed, Growing, Mature }
    public GrowStage currentGrowStage;

    Animator anim;
    [HideInInspector] public float growTime;

    [HideInInspector] public float initialWaitToGrowTime;

    [HideInInspector] public bool hasStartedAnimFinished;
    [HideInInspector] public bool hasStartedAnimReachedKeyMoment;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        currentGrowStage = GrowStage.Seed;
        anim.SetTrigger("Seed");
    }

    private void Start()
    { 
        StartCoroutine("InitialWaitToGrowTimer");
    }

    IEnumerator InitialWaitToGrowTimer()
    {
        yield return new WaitForSeconds(initialWaitToGrowTime);
        StartGrow();
    }

    public void StartGrow()
    {
        StartCoroutine("Grow");
    }

    IEnumerator Grow()
    {
        currentGrowStage = GrowStage.Growing;
        anim.speed = 1 / growTime;
        anim.SetTrigger("Grow");

        hasStartedAnimFinished = false;
        while (!hasStartedAnimFinished) { yield return null; }

        currentGrowStage = GrowStage.Mature;
        plantPot.AddMaturePlant();
        anim.speed = 1;
    }

    public void StartHarvest()
    {
        StartCoroutine("Harvest");
    }

    IEnumerator Harvest()
    {
        anim.speed = 1; //should be redundant
        anim.SetTrigger("Harvest");

        hasStartedAnimFinished = false;
        while (!hasStartedAnimFinished) { yield return null; }

        currentGrowStage = GrowStage.Seed;
        plantPot.SubtractMaturePlant();
        StartGrow();
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
