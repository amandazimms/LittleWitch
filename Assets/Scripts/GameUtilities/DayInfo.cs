using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayInfo : MonoBehaviour
{
    public int peasantWaitBeforeLeaveTime = 20;
    public float timeBetweenPeasantsAppearing;

    public float dayDifficultyMultiplier;

    public int nightLength = 10;
    public int dayLength = 10;
    public int sunriseSunsetLength = 5f;

    public bool stomachacheUnlocked = false;
    public bool headcoldUnlocked = false;

    public TimeOfDay currentTime;
    public enum TimeOfDay { Dawn, Day, Dusk, Night }

    public UnityEngine.Events.UnityEvent OnDawn;
    public UnityEngine.Events.UnityEvent OnDay;
    public UnityEngine.Events.UnityEvent OnDusk;
    public UnityEngine.Events.UnityEvent OnNight;

    public void Start() {
        Dusk();

    }

    public IEnumerator TimeCountdown(int lengthToCount){
        int currentCount = lengthToCount;

        while (currentCount > 0) {
            yield return new WaitForSeconds(1);
            currentCount--;
        }

        //day or night over, this will set it to dawn or dusk respectively
        SetTimeOfDay();
    }

    public void SetTimeOfDay() {

        switch (currentTime){
            case TimeofDay.Dusk:
                Night();
                break;
            case TimeofDay.Night:
                Dawn();
                break;
            case TimeofDay.Dawn:
                Day();
                break;
            case TimeofDay.Day:
                Dusk();
                break;
        }
    }

    public void Dusk(){
        currentTime = TimeofDay.Dusk;

        if (OnDusk != null)
            OnDusk.Invoke();

        StartCoroutine(TimeCountdown(sunriseSunsetLength));
    }
    public void Night() {
        currentTime = TimeofDay.Night;

        if (OnNight != null)
            OnNight.Invoke();

        StartCoroutine(TimeCountdown(nightLength));
    }
    public void Dawn() {
        currentTime = TimeofDay.Dawn;
        
        if (OnDawn != null)
            OnDawn.Invoke();
    }
    public void Day() {
        currentTime = TimeofDay.Day;
        
        if (OnDay != null)
            OnDay.Invoke();

        StartCoroutine(TimeCountdown(dayLength));

    }
}
