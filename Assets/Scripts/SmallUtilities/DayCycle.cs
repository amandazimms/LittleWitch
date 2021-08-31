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

    }

    public void DoNight()
    {
        //anim automatically goes to night after dusk
    }

    public void DoDawn()
    {
        anim.SetTrigger("Dawn");
    }

    public void DoDay()
    {

        //anim automatically goes to day after dawn
    }
}
