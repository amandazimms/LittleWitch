using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCustomizer : MonoBehaviour
{
    public float speedAdder = -.8f, speedMult = 4f;
    public Slider hueSlider, satSlider, valSlider;

    [Space(10)]

    public Slider strengthSlider;   
    public Slider speedSlider;

    [Space(10)]

    public InputField nameText;

    [Space(10)]

    public Animator anim;

    void Awake()
    {
        hueSlider.onValueChanged.AddListener(ChangeHue);
    }

    void ChangeHue(float hue)
    {
        /*
        currentColor = Color.HSVToRGB(hueSlider.value, satSlider.value, valSlider.value);

        foreach (SpriteRenderer rend in foxAvatarSprites)
            rend.color = currentColor;
            */
    }


    private void OnDestroy()
    {
        hueSlider.onValueChanged.RemoveAllListeners();
        satSlider.onValueChanged.RemoveAllListeners();
        valSlider.onValueChanged.RemoveAllListeners();
        strengthSlider.onValueChanged.RemoveAllListeners();
        speedSlider.onValueChanged.RemoveAllListeners();
        nameText.onValueChanged.RemoveAllListeners();
    }

    public void DoneButton()
    {
        //StaticData.PlayerStrength = strengthSlider.value;
        print("note - characterCustomizer.DoneButton() needs to be hooked up");
    }

}
