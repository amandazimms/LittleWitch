using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ReputationAvatar : MonoBehaviour
{
    Slider slider;
    public SpriteRenderer sliderColorRef; //e.g. use witch's hat or face color here, drag one of those sprites in inspector.

    RAColorChange[] colorChangers;
    RAPositionChange[] positionChangers;
    RAScaleChange[] scaleChangers;
    RARotationChange[] rotationChangers;
    RASpriteChange[] spriteChangers;

    GameObject gameManager;
    GameMaster gameMaster;

    void Awake()
    {
        gameManager = GameObject.FindWithTag("GameManager");
        gameMaster = gameManager.GetComponent<GameMaster>();

        slider = GetComponent<Slider>();
        colorChangers = GetComponentsInChildren<RAColorChange>();
        positionChangers = GetComponentsInChildren<RAPositionChange>();
        scaleChangers = GetComponentsInChildren<RAScaleChange>();
        rotationChangers = GetComponentsInChildren<RARotationChange>();
        spriteChangers = GetComponentsInChildren<RASpriteChange>();

        gameMaster.currentReputation = .5f; //why
    }

    void Update()
    {
        //todo move all these to events instead of update (when reputation moves, call the event)
        DoSpriteChanges();

        DoColorChanges();

        DoPositionChanges();

        DoScaleChanges();

        DoRotationChanges();

        UpdateSlider();
    }

    void DoSpriteChanges()
    {
        foreach (RASpriteChange spriteChanger in spriteChangers)
        {
            if (gameMaster.currentReputation < .2f)
                spriteChanger.spriteRend.sprite = spriteChanger.sprites[0];
            else if (gameMaster.currentReputation >= .2f && gameMaster.currentReputation < .4f)
                spriteChanger.spriteRend.sprite = spriteChanger.sprites[1];
            else if (gameMaster.currentReputation >= .4f && gameMaster.currentReputation < .6f)
                spriteChanger.spriteRend.sprite = spriteChanger.sprites[2];
            else if (gameMaster.currentReputation >= .6f && gameMaster.currentReputation < .8f)
                spriteChanger.spriteRend.sprite = spriteChanger.sprites[3];
            else if (gameMaster.currentReputation >= .8f)
                spriteChanger.spriteRend.sprite = spriteChanger.sprites[4];
        }
    }
    void DoColorChanges()
    {
        foreach (RAColorChange colorChanger in colorChangers)
        {
            if (gameMaster.currentReputation < .25f)
                colorChanger.spriteRend.color = Color.Lerp(colorChanger.colors[0], colorChanger.colors[1], gameMaster.currentReputation * 5);
            else if (gameMaster.currentReputation >= .25f && gameMaster.currentReputation < .5f)
                colorChanger.spriteRend.color = Color.Lerp(colorChanger.colors[1], colorChanger.colors[2], (gameMaster.currentReputation - .25f) * 5);
            else if (gameMaster.currentReputation >= .5f && gameMaster.currentReputation < .75f)
                colorChanger.spriteRend.color = Color.Lerp(colorChanger.colors[2], colorChanger.colors[3], (gameMaster.currentReputation - .5f) * 5);
            else if (gameMaster.currentReputation >= .75f)
                colorChanger.spriteRend.color = Color.Lerp(colorChanger.colors[3], colorChanger.colors[4], (gameMaster.currentReputation - .75f) * 5);
        }
    }
    void DoPositionChanges()
    {
        foreach (RAPositionChange posChanger in positionChangers)
        {
            if (gameMaster.currentReputation < .25f)
                posChanger.transform.localPosition = Vector3.Lerp(posChanger.positions[0], posChanger.positions[1], gameMaster.currentReputation * 5);
            else if (gameMaster.currentReputation >= .25f && gameMaster.currentReputation < .5f)
                posChanger.transform.localPosition = Vector3.Lerp(posChanger.positions[1], posChanger.positions[2], (gameMaster.currentReputation - .25f) * 5);
            else if (gameMaster.currentReputation >= .5f && gameMaster.currentReputation < .75f)
                posChanger.transform.localPosition = Vector3.Lerp(posChanger.positions[2], posChanger.positions[3], (gameMaster.currentReputation - .5f) * 5);
            else if (gameMaster.currentReputation >= .75f)
                posChanger.transform.localPosition = Vector3.Lerp(posChanger.positions[3], posChanger.positions[4], (gameMaster.currentReputation - .75f) * 5);
        }
    }
    void DoScaleChanges()
    {
        foreach (RAScaleChange scaleChanger in scaleChangers)
        {
            if (gameMaster.currentReputation < .25f)
                scaleChanger.transform.localScale = Vector3.Lerp(scaleChanger.scales[0], scaleChanger.scales[1], gameMaster.currentReputation * 5);
            else if (gameMaster.currentReputation >= .25f && gameMaster.currentReputation < .5f)
                scaleChanger.transform.localScale = Vector3.Lerp(scaleChanger.scales[1], scaleChanger.scales[2], (gameMaster.currentReputation - .25f) * 5);
            else if (gameMaster.currentReputation >= .5f && gameMaster.currentReputation < .75f)
                scaleChanger.transform.localScale = Vector3.Lerp(scaleChanger.scales[2], scaleChanger.scales[3], (gameMaster.currentReputation - .5f) * 5);
            else if (gameMaster.currentReputation >= .75f)
                scaleChanger.transform.localScale = Vector3.Lerp(scaleChanger.scales[3], scaleChanger.scales[4], (gameMaster.currentReputation - .75f) * 5);
        }
    }
    void DoRotationChanges()
    {
        foreach (RARotationChange rotationChanger in rotationChangers)
        {
            if (gameMaster.currentReputation < .25f)
            {
                Vector3 rot = Vector3.Lerp(rotationChanger.rotations[0], rotationChanger.rotations[1], gameMaster.currentReputation * 5);
                rotationChanger.transform.localRotation = Quaternion.Euler(rot.x, rot.y, rot.z);
            }
            else if (gameMaster.currentReputation >= .25f && gameMaster.currentReputation < .5f)
            {
                Vector3 rot = Vector3.Lerp(rotationChanger.rotations[1], rotationChanger.rotations[2], (gameMaster.currentReputation - .25f) * 5);
                rotationChanger.transform.localRotation = Quaternion.Euler(rot.x, rot.y, rot.z);
            }
            else if (gameMaster.currentReputation >= .5f && gameMaster.currentReputation < .75f)
            {
                Vector3 rot = Vector3.Lerp(rotationChanger.rotations[2], rotationChanger.rotations[3], (gameMaster.currentReputation - .5f) * 5);
                rotationChanger.transform.localRotation = Quaternion.Euler(rot.x, rot.y, rot.z);
            }
            else if (gameMaster.currentReputation >= .75f)
            {
                Vector3 rot = Vector3.Lerp(rotationChanger.rotations[3], rotationChanger.rotations[4], (gameMaster.currentReputation - .75f) * 5);
                rotationChanger.transform.localRotation = Quaternion.Euler(rot.x, rot.y, rot.z);
            }
        }
    }
    void UpdateSlider()
    {
        var sliderColors = slider.colors;
        sliderColors.disabledColor = sliderColorRef.color;
        slider.colors = sliderColors;

        slider.value = gameMaster.currentReputation;
    }
}
