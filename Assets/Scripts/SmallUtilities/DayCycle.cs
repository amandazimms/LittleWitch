using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayCycle : MonoBehaviour
{
    public GameObject gameManager;
    public DayInfo dayInfo;

    public float dawnDuskLength;
    public Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        gameManager = GameObject.FindWithTag("GameManager");
        dayInfo = gameManager.GetComponent<DayInfo>();

        dayInfo.OnDay.AddListener(DoDay);
        dayInfo.OnDusk.AddListener(DoDusk);
        dayInfo.OnNight.AddListener(DoNight);
        dayInfo.OnDawn.AddListener(DoDawn);

        dawnDuskLength = dayInfo.dawnDuskLength;
        anim.speed = 1 / dawnDuskLength;
    }

    public void DoDusk()
    {
        anim.SetTrigger("Dusk");
        print("dusk");

    }

    public void DoNight()
    {
        print("night");

        //anim automatically goes to night after dusk
    }

    public void DoDawn()
    {
        print("dawn");

        anim.SetTrigger("Dawn");
    }

    public void DoDay()
    {
        print("day");


        //anim automatically goes to day after dawn
    }
}
