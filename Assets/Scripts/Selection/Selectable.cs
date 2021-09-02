using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Rendering;

[RequireComponent(typeof(SortingGroup))]
public class Selectable : MonoBehaviour
{
    /// every selectable object in game needs one of these. 

    /// every GO with this script on it needs to be on the layer "Selectable". 
    /// any other colliders in the GO's hierarchy (i.e. normal colliders for running into things)...
    /// ...need to NOT be on the Selectable layer - 
    /// - the GOs with those colliders should be on (a) Default layer if the GO moves (witch, peasant)
    /// , or (b) the CollidersForMap layer if it doesnt move (house, bush).

    /// this does: (1) sets the name of that thing (for NameDisplay to use)
    /// (2) tells whether this thing is currently Selected or moused over
    /// (3) does glow effect if it is selected

    public bool isColorChanging = false;
    public SpriteRenderer[] mySprites; 
    public List<Color> myStartColors;


    public bool isSelected = false; //just for debugging since nothing relies on this?
    public bool isMousedOver = false; //just for debugging since nothing relies on this?

    [HideInInspector] public string displayedName; //ResourceStats script will populate this

    SelectionManager selectionManager;

    [HideInInspector] public Vector3 menuOffset;

    public UnityEngine.Events.UnityEvent OnASelected;
    public UnityEngine.Events.UnityEvent OnADeselected;



    public bool overRideOriginalAlphaTo1 = false;
    public bool isCurrentlyUnselectable; 

    void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Selectable"); //this should be set in inspector anyway but just making sure. 

        GameObject gameManager = GameObject.FindWithTag("GameManager");

        selectionManager = gameManager.GetComponent<SelectionManager>();
        selectionManager.population.Add(this);

        StartCoroutine(LateStart());
    }

    public void CalculateSprites()
    {
        mySprites = null;
        mySprites = GetComponentsInChildren<SpriteRenderer>(false);
    }

    IEnumerator LateStart() //If we just put this in Start, colorSeasons runs after this and we don't get the correct color. With this little wait, the problem is fixed.
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(1f);

        CalculateSprites();

        for (int i = 0; i < mySprites.Length; i++)
        {
            if (mySprites[i] != null) // && mySprites[i].enabled == true)
            {
                if (overRideOriginalAlphaTo1)
                {
                    SpriteRenderer oSpriteRend = mySprites[i];
                    Color adjustedColor = oSpriteRend.color;
                    adjustedColor.a = 1;
                    myStartColors.Add(adjustedColor);
                }
                else
                {
                    SpriteRenderer spriteRend = mySprites[i];
                    myStartColors.Add(spriteRend.color);
                }
            }
        }

        if (isColorChanging)
            GetUnselectedColor();
    }

    public IEnumerator SelectionGlow()
    {
        if (isColorChanging)
            GetUnselectedColor();

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

        if (!isCurrentlyUnselectable)
            StopSelectionGlow();
    }

    public void SelectMe()
    {
        if (!isCurrentlyUnselectable) {

            if (OnASelected != null)
                OnASelected.Invoke();

            isSelected = true;

            StartCoroutine("SelectionGlow");
        }
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


   //todo - delete this function? not needed?
    void GetUnselectedColor() //need to update the startColor as seasons change, but doesn't need to be every frame since they change slowly
    {
        for (int i = 0; i < mySprites.Length; i++)
        {
            if (mySprites[i] != null)
            {
                SpriteRenderer spriteRend = mySprites[i];
                myStartColors[i] = spriteRend.color;
            }
        }

    }
}
