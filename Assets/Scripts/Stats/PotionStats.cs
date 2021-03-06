using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionStats : MonoBehaviour
{
    public GameObject cork;
    public GameObject glow;

    [Space(5)]

    public SpriteMask liquidSpritemask;
    public SpriteRenderer liquidSpriteRenderer;

    public Sprite[] liquidDraining;

    public Animator anim;

    public void Start()
    {
        
    }

    public IEnumerator DrinkAnim(float totalSeconds) //called from PeasantStats.GivePotion.
    {
        //note - this anim only messes with the glow, the liquid draining part is handled below
        anim.SetTrigger("Drink");

        float secondsInterval = totalSeconds / liquidDraining.Length;

        liquidSpritemask.sprite = liquidDraining[0]; //all the way full sprite - should be this already
        liquidSpriteRenderer.sprite = liquidDraining[0];

        yield return new WaitForSeconds(secondsInterval);
        liquidSpritemask.sprite = liquidDraining[1];
        liquidSpriteRenderer.sprite = liquidDraining[1];

        yield return new WaitForSeconds(secondsInterval);
        liquidSpritemask.sprite = liquidDraining[2];
        liquidSpriteRenderer.sprite = liquidDraining[2];

        yield return new WaitForSeconds(secondsInterval);
        liquidSpritemask.sprite = liquidDraining[3];
        liquidSpriteRenderer.sprite = liquidDraining[3];

        yield return new WaitForSeconds(secondsInterval);
        liquidSpritemask.sprite = liquidDraining[4];
        liquidSpriteRenderer.sprite = liquidDraining[4];

        yield return new WaitForSeconds(secondsInterval);
        liquidSpritemask.sprite = liquidDraining[5];
        liquidSpriteRenderer.sprite = liquidDraining[5];

        yield return new WaitForSeconds(secondsInterval);
        liquidSpritemask.sprite = liquidDraining[6];
        liquidSpriteRenderer.sprite = liquidDraining[6];

        yield return new WaitForSeconds(secondsInterval);
        liquidSpritemask.sprite = liquidDraining[7];
        liquidSpriteRenderer.sprite = liquidDraining[7];

        yield return new WaitForSeconds(secondsInterval);
        liquidSpritemask.sprite = liquidDraining[8];
        liquidSpriteRenderer.sprite = liquidDraining[8];

        yield return new WaitForSeconds(secondsInterval);
        liquidSpritemask.sprite = liquidDraining[9];
        liquidSpriteRenderer.sprite = liquidDraining[9];

        yield return new WaitForSeconds(secondsInterval);
        DeactivateSpriteRendererAndMask();

        Destroy(glow);
    }

    public IEnumerator FillAnim(float totalSeconds) //called from PeasantStats.GivePotion.
    {
        //note - this anim only messes with the glow, the liquid draining part is handled below
        anim.SetTrigger("Fill");

        liquidSpritemask.sprite = liquidDraining[9]; //all the way empty sprite - (is not this already)
        liquidSpriteRenderer.sprite = liquidDraining[9];

        //waiting for 'dead time' in animation
        yield return new WaitForSeconds(1.08f);
            

        float secondsInterval = totalSeconds / liquidDraining.Length;

        liquidSpritemask.sprite = liquidDraining[9]; //all the way empty sprite - (is not this already)
        liquidSpriteRenderer.sprite = liquidDraining[9];

        liquidSpritemask.sprite = liquidDraining[8]; 
        liquidSpriteRenderer.sprite = liquidDraining[8];

        yield return new WaitForSeconds(secondsInterval);
        liquidSpritemask.sprite = liquidDraining[7];
        liquidSpriteRenderer.sprite = liquidDraining[7];

        yield return new WaitForSeconds(secondsInterval);
        liquidSpritemask.sprite = liquidDraining[6];
        liquidSpriteRenderer.sprite = liquidDraining[6];

        yield return new WaitForSeconds(secondsInterval);
        liquidSpritemask.sprite = liquidDraining[5];
        liquidSpriteRenderer.sprite = liquidDraining[5];

        yield return new WaitForSeconds(secondsInterval);
        liquidSpritemask.sprite = liquidDraining[4];
        liquidSpriteRenderer.sprite = liquidDraining[4];

        yield return new WaitForSeconds(secondsInterval);
        liquidSpritemask.sprite = liquidDraining[3];
        liquidSpriteRenderer.sprite = liquidDraining[3];

        yield return new WaitForSeconds(secondsInterval);
        liquidSpritemask.sprite = liquidDraining[2];
        liquidSpriteRenderer.sprite = liquidDraining[2];

        yield return new WaitForSeconds(secondsInterval);
        liquidSpritemask.sprite = liquidDraining[1];
        liquidSpriteRenderer.sprite = liquidDraining[1];

        yield return new WaitForSeconds(secondsInterval);
        liquidSpritemask.sprite = liquidDraining[0];
        liquidSpriteRenderer.sprite = liquidDraining[0];

        yield return new WaitForSeconds(secondsInterval);
    }

    public void DeactivateSpriteRendererAndMask()
    {
        liquidSpriteRenderer.enabled = false;
        liquidSpritemask.enabled = false;
    }

    public void Uncork()
    {
        StartCoroutine(UncorkCR());
    }

    IEnumerator UncorkCR()
    {
        anim.SetTrigger("Uncork");
        yield return new WaitForSeconds(.55f);
        Destroy(cork);
    }
}
