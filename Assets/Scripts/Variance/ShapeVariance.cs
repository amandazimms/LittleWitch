using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShapeVariance : MonoBehaviour 
{
    /// I feel this name is a little inexact but can't come up with a better one.
    /// this script swaps out the sprite for another one. 
    /// e.g. make 3 art assets for flower heads / petals and load this script up with those sprites.
    /// now each flower that spawns from that prefab will have a different randomized head.
    /// yields pretty unique objects if you make several of each part; e.g. 3 each flower heads, stems, and leaves = 27 flower combos.
    /// combine that with color and transform variance and the world looks pretty special. :D

    [Header("Standard")]
    public SpriteRenderer myShapeSpriteRenderer;
    public Image myImage; 
    public Sprite[] shapeOptions;
    public Sprite chosenShapeSprite;

    [Header("Sprite Masks")]
    [Tooltip("Use Sprite Masks - e.g. Pond")]
    public bool useSpriteMasks;
    public SpriteMask mySpriteMask;

    [Header("Inherit From Other ShapeVariance")]
    [Tooltip("Use when you have multiple objects you want shape variance for, but their sprite choice should be linked; e.g. leaves on trees can choose either 3, 3a, and 3b, or 4, 4a, and 4b - not 3, 4a, and 3b")]
    public bool doInherit = false;
    [Tooltip("Put shapeOptions in the same order on this SV as inherited SV - e.g. 3, 4, 5 and 3a, 4a, 5a. 3s will be paired")]
    public ShapeVariance shapeVarianceToInheritFrom;
 
    [Header("Hidden Objects")]
    [Tooltip("Use when you want to disable another spriteRenderer (spriteToHide) if a certain sprite shape (shapeOptionsIndexesToHide) is chosen")]
    public bool hiddenObjectSpecialCase = false; //this is only used for flowers, wanted to hide the "center" part if it's a tulip or dandelion.
    public int[] shapeOptionsIndexesToHide;
    public SpriteRenderer spriteToHide;

    [Header("Colliders")]
    [Tooltip("Use if you want the script to add a polygon collider to the object after the sprite is chosen. Don't check true for both TransformVariance and ShapeVariance Scripts, just one.")]
    public bool addPolygonCollider;
    [Tooltip("Check true for collision; false for selection")]
    public bool colliderIsForCollision = false;

    [HideInInspector] public int spriteNumberChosen; //used by ObjectPlacer script

    void Awake()
    {
        if (!doInherit)
            RollRandoms();
    }

    void Start()
    {
        if (doInherit) //needs to be in Start so Inherit-parent's Awake runs first
            GetRandomsFromInherited();

        SetShape(); //needs to be in Start for same reason as above.

        if (useSpriteMasks == true)
            SetMask();

        if (hiddenObjectSpecialCase)
            HideObjects();

        if (addPolygonCollider) //note that we are also counting on this to run after TransformVariance's stuff finishes in its Awake function.
            AddPolygonCollider();
    }

    public void RollRandoms()
    {
        spriteNumberChosen = Random.Range(0, shapeOptions.Length);
        chosenShapeSprite = shapeOptions[spriteNumberChosen];  //use above random number to point to a member of the shapeOptions array, and call it chosenShapeSprite
    }

    void GetRandomsFromInherited()
    {
        spriteNumberChosen = shapeVarianceToInheritFrom.spriteNumberChosen;
        chosenShapeSprite = shapeOptions[spriteNumberChosen];  //use above number to point to a member of the shapeOptions array, and call it chosenShapeSprite
    }

    public void SetShape()
    {
        if (myShapeSpriteRenderer != null)
            myShapeSpriteRenderer.sprite = chosenShapeSprite; //set our sprite to that shape

        if (myImage != null)
        {
            Vector3 originalScale = myImage.rectTransform.sizeDelta; //get the original size of the button and store
            myImage.sprite = chosenShapeSprite; //swap the sprite out for the new one
            myImage.rectTransform.sizeDelta = originalScale; //assign the new sprite's button size to the stored size of the old one
        }
    }

    void HideObjects()
    {
        foreach (int index in shapeOptionsIndexesToHide)
            if (spriteNumberChosen == index)
                spriteToHide.enabled = false;
    }

    void SetMask()
    {
        mySpriteMask.sprite = chosenShapeSprite; //make the mask match the shape
    }

    void AddPolygonCollider()
    {
        PolygonCollider2D newCollider = gameObject.AddComponent<PolygonCollider2D>();
        newCollider.usedByComposite = true;
        if (colliderIsForCollision)
            newCollider.isTrigger = true;
    }

}
