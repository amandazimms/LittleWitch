using UnityEngine;
using System.Collections.Generic;

//[ExecuteInEditMode] //todo remove for final build
public class MasterColorVarianceSprites : MonoBehaviour
{
    /// this must live on the GameManager object, with that tag.
    /// it collects all of the ColorVarianceSprites scripts in the scene 
    /// (actually they "collect themselves" and add them to it, via the GameManager tag)
    /// it makes all of the sprites-changing-colors-with-the-time-of-day logic run here in a for loop...
    /// ...instead of 1,000,000 times in each individual script on each individual object.
    /// if any of those collected CVS scripts are using ColorSeasons, the logic is done in MasterColorSeasonsSprites instead

    public List<ColorVarianceSprites> CVSList;

    bool noTimesMode;

    public bool debugRerollAllColorsInScene = false; //todo remove for final build

    public void Awake()
    {
    }

    public void Start()
    {
    }

    public void Update()
    {
        UpdateTint();

        if (debugRerollAllColorsInScene)
            Reroll(); //remove for final build
    }

    public void Reroll() 
    {
        ColorVarianceSprites[] allCVSsInScene = FindObjectsOfType<ColorVarianceSprites>();
        foreach (ColorVarianceSprites cvs in allCVSsInScene)
            cvs.Reroll();
     
        debugRerollAllColorsInScene = false;
    }

    public void UpdateTint()
    {
        for (int i = 0; i < CVSList.Count; i++)
            if (CVSList[i] != null)
                foreach (SpriteRenderer spriteRend in CVSList[i].mySprites)
                    if (spriteRend != null)
                        spriteRend.color = CVSList[i].finalColor;
    }
}
