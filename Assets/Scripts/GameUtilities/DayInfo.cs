using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayInfo : MonoBehaviour
{
    [Header("Day Length Settings")]
    [Tooltip("Set this with day '-1' in mind. On the very first night, the extraLengthPerNight will also get added.")]
    public int currentNightLength = 8;
    public int dayLength = 6;
    public int dawnDuskLength = 10;
    [Tooltip("Every subsequent night, the nightLength will be this many seconds longer.")]
    public int extraLengthPerNight = 4;

    [Space(5)]

    [Tooltip("how many nights in this game? e.g. end after this many nights have completed")]
    public int finalNight = 10;

    [Space(5)]

    [Header("Current info")]
    public int nightCount = 0;

    public TimeOfDay currentTime;
    public enum TimeOfDay { Dawn, Day, Dusk, Night }

    public UnityEngine.Events.UnityEvent OnDawn;
    public UnityEngine.Events.UnityEvent OnDay;
    public UnityEngine.Events.UnityEvent OnDusk;
    public UnityEngine.Events.UnityEvent OnNight;
    public UnityEngine.Events.UnityEvent OnFinalMorning;

    public void Start() {
        SetDusk();
    }

    public IEnumerator TimeOfDayCountdown(int lengthToCount){
        int currentCount = lengthToCount;

        while (currentCount > 0) {
            yield return new WaitForSeconds(1);
            currentCount--;
        }

        AdvanceTimeOfDay();
    }

    public void AdvanceTimeOfDay() {

        switch (currentTime){
            case TimeOfDay.Dusk:
                SetNight();
                break;
            case TimeOfDay.Night:
                SetDawn();
                break;
            case TimeOfDay.Dawn:
                SetDay();
                break;
            case TimeOfDay.Day:
                SetDusk();
                break;
        }
    }

    public void SetDusk(){
        currentTime = TimeOfDay.Dusk;

        if (OnDusk != null)
            OnDusk.Invoke();

        StartCoroutine(TimeOfDayCountdown(dawnDuskLength));
    }
    public void SetNight() {
        currentTime = TimeOfDay.Night;
        nightCount++;
        currentNightLength += extraLengthPerNight;

        if (OnNight != null)
            OnNight.Invoke();

        StartCoroutine(TimeOfDayCountdown(currentNightLength));
    }
    public void SetDawn() {
        currentTime = TimeOfDay.Dawn;

        if (OnDawn != null)
            OnDawn.Invoke();

        StartCoroutine(TimeOfDayCountdown(dawnDuskLength));
    }
    public void SetDay() {
        currentTime = TimeOfDay.Day;

        if (OnDay != null)
            OnDay.Invoke();

        if (nightCount != finalNight) //if it's any other day...
            StartCoroutine(TimeOfDayCountdown(dayLength));
        else //if it's the final day and the game is over...
        {
            if (OnFinalMorning != null)
                OnFinalMorning.Invoke();
        }
    }
}
