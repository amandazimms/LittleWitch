using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HutSwitcher : MonoBehaviour
{
    public SpriteRenderer[] spritesToHide;
    public GameObject cauldronObject;
    public SpriteRenderer blackoutSprite;
    SpriteRenderer[] cauldronSprites;

    public List<SpriteRenderer> allFadingSprites;

    public float totalFadeSeconds;
    public float currentFadeSeconds;
    public float lerp;

    private void Awake()
    {
        foreach (SpriteRenderer sprite in spritesToHide)
            allFadingSprites.Add(sprite);

        cauldronSprites = cauldronObject.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sprite in cauldronSprites)
            allFadingSprites.Add(sprite);

        SetInitialAlphas();
    }

    public void SetInitialAlphas()
    {
        foreach (SpriteRenderer sprite in allFadingSprites)
        {
            Color newColor = sprite.color;
            newColor.a = 0;
            sprite.color = newColor;
        }
        Color bNewColor = blackoutSprite.color;
        bNewColor.a = 0;
        blackoutSprite.color = bNewColor;
    }

    IEnumerator LerpIncrease()
    {
        currentFadeSeconds = 0;

        while (currentFadeSeconds < totalFadeSeconds)
        {
            currentFadeSeconds += Time.deltaTime;
            lerp = currentFadeSeconds / totalFadeSeconds;
            yield return null;
        }
    }


    public void SwitchToInside()
    {
        StartCoroutine("LerpIncrease");
        StartCoroutine("GoInside");
    }

    IEnumerator GoInside()
    {
        while (currentFadeSeconds < totalFadeSeconds)
        {
            foreach (SpriteRenderer sprite in allFadingSprites)
            {
                Color newColor = sprite.color;
                newColor.a = Mathf.Lerp(0, 1, lerp);
                sprite.color = newColor;
            }

            Color bNewColor = blackoutSprite.color;
            bNewColor.a = Mathf.Lerp(0, .7f, lerp);
            blackoutSprite.color = bNewColor;

            yield return null;
        }
    }

    public void SwitchToOutside()
    {
        StartCoroutine("LerpIncrease");
        StartCoroutine("GoOutside");
    }

    IEnumerator GoOutside()
    {
        while (currentFadeSeconds < totalFadeSeconds)
        {
            foreach (SpriteRenderer sprite in allFadingSprites)
            {
                Color newColor = sprite.color;
                newColor.a = Mathf.Lerp(1, 0, lerp);
                sprite.color = newColor;
            }
            Color bNewColor = blackoutSprite.color;
            bNewColor.a = Mathf.Lerp(.7f, 0, lerp);
            blackoutSprite.color = bNewColor;

            yield return null;
        }
    }
}
