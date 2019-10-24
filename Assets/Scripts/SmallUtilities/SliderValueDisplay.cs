using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderValueDisplay : MonoBehaviour
{
    public Text valueText;
    public Slider slider;
    [Tooltip("Displayed value is multiplied by this")]
    public float multiplier;
    [Tooltip("Displayed value is this plus value * multiplier")]
    public float adder;
    [Tooltip("If true, value is rounded to int. Otherwise, it's rounded to 1 decimal place. ")]
    public bool roundToInt;

    void Update()
    {
        if (!roundToInt)
            valueText.text = ((adder + slider.value) * multiplier).ToString("F1");
        else
            valueText.text = Mathf.RoundToInt((adder + slider.value) * multiplier).ToString();

    }
}
