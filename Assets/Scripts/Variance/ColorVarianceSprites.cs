using UnityEngine;
using System.Collections.Generic;

//[ExecuteInEditMode] //todo remove for final build
public class ColorVarianceSprites : MonoBehaviour
{
    ///these go on pretty much every sprite in the game unless:
    ///(1) it is using ColorSeasonsSprites AND
    ///(2) -and- it doesn't use any varience.
    /// eg I believe the background grass is this, but for sprites. I don't think there are any for sprites.
    /// the beginning logic for what individual colors it will be is calculated here,
    /// then MasterColorVarianceSprites handles the updating-color-each-frame on the GameManager object.


    MasterColorVarianceSprites masterCVS;

    public string title; 

    [Header("I Want To Manually Drag...")]
    public bool specificSprites = false;
    public SpriteRenderer[] myGroupSpriteRenderers;

    [Header("I Want To SemiAutomatically Use...")]
    public bool colorFlagging = false;
    public enum flagLetter { A, B, C, D, E };
    public flagLetter myFlag;

    [Header("I Want To Automatically Use...")]
    public bool allChildSprites = false;

    [Tooltip("Lo represents have low end of hues, sats, and vals for possible end color. Hi for high end.")]
    public Color colorRangeLo, colorRangeHi;
    [Tooltip("Any colors in this range get excluded. Usually best to just speficy hues and leave values equal. Otherwise, works like above.")]
    public Color exColorRangeLo, exColorRangeHi;

    public Color finalColor; //final color that gets chosen by the randomizaton script
    float hueLo, hueHi, satLo, satHi, valLo, valHi, hueExcLo, hueExcHi; //possible range for each aspect of the final color. exc = values to be excluded
    float finalHue, finalSat, finalVal; //chosen from within the hueLo, hueHi... etc, final values chosen.
    float myBaselineAlpha; //need this so that when assigning a varied color, it doesn't automatically change alpha to 100

    public SpriteRenderer[] mySprites;

    public bool debugRerollColors = false; //todo remove for final build
    public bool debugRerollAllColors = false; //todo remove for final build

    public bool dontSetMyStartColor = false; //used for 1st gen player (since this is set in customization menu)

    public void Start()
    {
        GameObject gameManager = GameObject.FindWithTag("GameManager");
        masterCVS = gameManager.GetComponent<MasterColorVarianceSprites>();

        masterCVS.CVSList.Add(this);

        SetSprites();

        if (!dontSetMyStartColor)
            SetMyStartColor();
    }

    private void Update()
    {
        /*
        if (debugRerollColors == true) //todo for testing only
            Reroll();

        if (debugRerollAllColors == true) //todo for testing only
            RerollAll();
            */
    }

    public void Reroll()
    {
        Start();
        masterCVS.Awake();
        masterCVS.Start();
        masterCVS.UpdateTint();

        debugRerollColors = false;
    }

    public void RerollAll()
    {
        ColorVarianceSprites[] allCVSs = GetComponentsInChildren<ColorVarianceSprites>();
        foreach (ColorVarianceSprites cvs in allCVSs)
            cvs.Start();

        masterCVS.Awake();
        masterCVS.Start();
        masterCVS.UpdateTint();

        debugRerollAllColors = false;
    }

    void SetSprites()
    {
        if (specificSprites == true)
            mySprites = myGroupSpriteRenderers;

        else if (colorFlagging == true)
        {
            List<SpriteRenderer> addingSpritesList = new List<SpriteRenderer>(); //empty list of sprite renderers

            ColorFlag[] allColorFlags = GetComponentsInChildren<ColorFlag>(); //(full) array of all the ColorFlags in this GO's child hierarchy

            foreach (ColorFlag flag in allColorFlags) //cycle through the above array
            {
                if ( (flag.flagA && myFlag == flagLetter.A) || (flag.flagB && myFlag == flagLetter.B) || (flag.flagC && myFlag == flagLetter.C) || (flag.flagD && myFlag == flagLetter.D) || (flag.flagE && myFlag == flagLetter.E)) //if it's the same flag I have selected for myFlag
                {
                    SpriteRenderer spriteR = flag.GetComponent<SpriteRenderer>(); //get the Sprite Renderer on the same GO
                    addingSpritesList.Add(spriteR); //and add that sprite renderer to our list
                }
            }

            mySprites = addingSpritesList.ToArray(); //now that the list is full, convert it to an array 
        }

        else if (allChildSprites == true)
            mySprites = GetComponentsInChildren<SpriteRenderer>();


        else
            print(gameObject + "'s Color Seasons has neither allChildSprites, colorFlagging, nor specificSprites selected");
    }

    void SetMyStartColor()
    {
        NumberfyInspectorColors();
        HueSelect();
        SetAlpha();
    }

    void SetAlpha()
    {
        foreach (SpriteRenderer spriteRend in mySprites)
        {
            myBaselineAlpha = spriteRend.color.a; //remove these to speed up performance - shouldn't need them?
            finalColor.a = myBaselineAlpha; 
        }
    }

    void NumberfyInspectorColors() //breaks down the inspector-set blocks of color (colorRangeLo...) into their HSV components.
    {
        float unusedA, unusedB, unusedC, unusedD; //the RGBToHSV function needs to generate all 3 HSL values, but for the Excl ones we only use hue. So we just put them in unused variables.

        Color.RGBToHSV(colorRangeLo, out hueLo, out satLo, out valLo);
        Color.RGBToHSV(colorRangeHi, out hueHi, out satHi, out valHi);

        Color.RGBToHSV(exColorRangeLo, out hueExcLo, out unusedA, out unusedB);
        Color.RGBToHSV(exColorRangeHi, out hueExcHi, out unusedC, out unusedD);
    }

    public void HueSelect()
    {
        finalHue = Random.Range(hueLo, hueHi);

        //need the 0 check because in many cases there will be no hues to exclude, thus they will be 0, and this part should be skipped
        if (hueExcHi != 0 && hueExcLo != 0 && finalHue >= hueExcLo && finalHue <= hueExcHi) // if the final hue chosen is in the "excl" range specified in inspector, reroll
            HueSelect();

        else
            SatAndValSelect();
    }

    public void SatAndValSelect()
    {
        finalSat = Random.Range(satLo, satHi);
        finalVal = Random.Range(valLo, valHi);

        finalColor = Color.HSVToRGB(finalHue, finalSat, finalVal);
    }

    void OnDestroy()
    {
        masterCVS.CVSList.Remove(this);
    }
}
