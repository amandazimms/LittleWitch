using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class Selectable : MonoBehaviour
{
    /// every selectable object in game needs one of these. 
    /// every GO with this script on it needs to be on the layer "Selectable". 
    /// any other colliders in the GO's hierarchy (i.e. normal colliders for running into things)...
    /// ...need to NOT be on the Selectable layer - 
    /// - the GOs with those colliders should be on (a) Default layer if the GO moves (Fox, raccoon)
    /// , or (b) the CollidersForMap layer if it doesnt move (Tree, berries).
    /// this does: (1) sets the name of that thing (for NameDisplay to use)
    /// (2) tells whether this thing is currently Selected or moused over
    /// (3) does glow effect if it is selected

    public bool isColorChanging = true;
    public SpriteRenderer[] mySprites; 
    [HideInInspector] public List<Color> myStartColors;


    public bool isSelected = false; //just for debugging since nothing relies on this?
    public bool isMousedOver = false; //just for debugging since nothing relies on this?

    [HideInInspector] public string displayedName; //ResourceStats script will populate this

    SelectionManager selectionManager;

    [HideInInspector] public Vector3 menuOffset;

    public UnityEngine.Events.UnityEvent OnASelected;
    public UnityEngine.Events.UnityEvent OnADeselected;

    void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Selectable"); //this should be set in inspector anyway but just making sure. 

        GameObject gameManager = GameObject.FindWithTag("GameManager");

        selectionManager = gameManager.GetComponent<SelectionManager>();
        selectionManager.population.Add(this);

        CalculateSprites();
        StartCoroutine(LateStart());
    }

    public void CalculateSprites()
    {
        mySprites = null;
        mySprites = GetComponentsInChildren<SpriteRenderer>();
    }

    IEnumerator LateStart() //If we just put this in Start, colorSeasons runs after this and we don't get the correct color. With this little wait, the problem is fixed.
    {
        yield return new WaitForEndOfFrame();

        for (int i = 0; i < mySprites.Length; i++)
        {
            if (mySprites[i] != null && mySprites[i].enabled == true)
            {
                SpriteRenderer spriteRend = mySprites[i];
                myStartColors.Add(spriteRend.color);
            }
        }

        if (isColorChanging)
            StartCoroutine(GetColor());
    }

    public IEnumerator SelectionGlow()
    {
        while (true)
        {
            for (int i = 0; i < mySprites.Length; i++) //flash all the sprites between normal color and glow color
            {
                if (mySprites[i] != null && mySprites[i].enabled == true)
                {
                    SpriteRenderer spriteRend = mySprites[i];
                    spriteRend.color = Color.Lerp(myStartColors[i], selectionManager.glowColor, Mathf.PingPong(Time.time, selectionManager.glowSpeed));
                }
            }
            yield return null;
        }
    }

    public void StopSelectionGlow()
    {
        StopCoroutine("SelectionGlow");
        for (int i = 0; i < mySprites.Length; i++) //return to normal color
        {
            if (mySprites[i] != null && mySprites[i].enabled == true)
            {
                SpriteRenderer spriteRend = mySprites[i];
                spriteRend.color = myStartColors[i];
            }
        }
    }

    public void DeselectMe()
    {
        if (OnADeselected != null)
            OnADeselected.Invoke();

        isSelected = false;
        StopSelectionGlow();
    }

    public void SelectMe()
    {
        if (OnASelected != null)
            OnASelected.Invoke();

        isSelected = true;
        StartCoroutine("SelectionGlow");
    }

    public void OnMouseEnter()
    {
        isMousedOver = true;
        selectionManager.MouseEnter(this);
    }

    public void OnMouseExit()
    {
        isMousedOver = false;
        selectionManager.MouseExit();
    }


    // this whole situation is a little more complicated for my game, because I have *every* sprite in the game slowly
    // changing colors throughout the season. 
    // if you had an object selected for long enough, the color it started at would be different from the one it currently is, 
    // and this would mess up the color it's supposed to be according to the season. 
    // this method below fixes that

    IEnumerator GetColor() //need to update the startColor as seasons change, but doesn't need to be every frame since they change slowly
    {
        yield return null;
        /*
        if (!isSelected)
        {
            for (int i = 0; i < mySprites.Length; i++)
            {
                if (mySprites[i] != null && mySprites[i].enabled == true)
                {
                    SpriteRenderer spriteRend = mySprites[i];
                    myStartColors[i] = spriteRend.color;
                }
            }
        }
        yield return null;

        int colorCountdown = Mathf.RoundToInt(seasons.seasonLength / 500); //if we do it ~500x per season, this only has to run like 
                                                                           //every 15-30 seconds (depends on seasonLength). The color difference between this and doing it constantly will be negligible
        if (colorCountdown < 1) //just in case we set very short seasons e.g. for testing, still want to wait at least 1 second per color calculation
            colorCountdown = 1;

        while (colorCountdown > 0)
        {
            yield return new WaitForSeconds(1);
            colorCountdown--;
        }

        StartCoroutine(GetColor());
        */
    }
}
