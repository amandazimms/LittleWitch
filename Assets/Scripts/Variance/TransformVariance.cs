using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformVariance : MonoBehaviour
{
    ///this changes up aspects of the transform on a prefab so no two are alike.
    ///can change the overall size (overall scale multiplier), or stretch/squash (width /height variance), or both.
    ///also rotate, flip (straightforward
    ///note that the first two, xVar and yVar, actually change the position of the prefab...
    /// ...this is useful for child objects - move the berries around on the bush
    /// ...it's also used on POIs in world generation to create the non-grid effect


    public float xVariance, yVariance; //for changing X and Y position
    public float rotateVariance; //for changing rotation (Z axis for normal 2D rotation)

    [Tooltip("How much Width and Height should vary. Can strech/skew the object")]
    public float widthVariance, heightVariance; 

    [Tooltip("PERCENTAGE MULTIPLIER for how much overall scale should vary, i.e. .1 will yield randoms between .9 and 1.1 * the scale. Will scale x&y proportionately, provided above two vars are 0")]
    public float overallScaleMultiplier; 

    public bool allowXFlipping; //is the object allowed to be flipped on the X axis?
    public Transform myTransform;

    [Header("Colliders")]
    [Tooltip("Check this if you want the script to add a polygon collider to the object after the transform is varied. Don't check true for both TransformVariance and ShapeVariance Scripts, just one.")]
    public bool addPolygonCollider;
    [Tooltip("Check true for collision; false for selection")]
    public bool colliderIsForCollision = false; 

    float xPosLo, xPosHi, yPosLo, yPosHi, widthLo, widthHi, heightLo, heightHi, rotateLo, rotateHi, scaleLo, scaleHi;
    Vector3 myTransformPos;


    [HideInInspector] public Vector3 originalScale; //how big was the prefab set to, before this script messed with it? most are 1 but some might be eg 1.2.

    void Awake()
    {
        originalScale = transform.localScale;

        myTransformPos = myTransform.localPosition;

        CalcPossiblePositions();
        CalcPossibleRotations();
        CalcPossibleScales();

        SetPosition();
        SetRotation();
        SetScale();

        TellScaleToStatsAndWander();
    }

    public void CalcPossiblePositions() //use single user inputted variable to generate 1 min and 1 max for Position (for x and y)
    {
        xPosLo = myTransformPos.x - xVariance;
        xPosHi = myTransformPos.x + xVariance;

        yPosLo = myTransformPos.y - yVariance;
        yPosHi = myTransformPos.y + yVariance;
    }

    public void CalcPossibleRotations() //use single user inputted variable to generate 1 min and 1 max for Rotation 
    {
        rotateLo = myTransform.localEulerAngles.z - rotateVariance;
        rotateHi = myTransform.localEulerAngles.z + rotateVariance;
    }

    public void CalcPossibleScales() //use single user inputted variable to generate 1 min and 1 max for Scale (for width and height)
    {
        widthLo = myTransform.localScale.x - widthVariance;
        widthHi = myTransform.localScale.x + widthVariance;

        heightLo = myTransform.localScale.y - heightVariance;
        heightHi = myTransform.localScale.y + heightVariance;

        scaleLo = 1 - overallScaleMultiplier; //so if you put in .1, you get .9    Or in .2, you get .8    Or in 0, get 1
        scaleHi = 1 + overallScaleMultiplier; //so if you put in .1, you get 1.1   Or in .2, you get 1.2   Or in 0, get 1
    }


    private void Start()
    {
        if (addPolygonCollider) //note that we are also counting on this to run after ShapeVariance's stuff finishes in its Awake function.
        {
            PolygonCollider2D newCollider = gameObject.AddComponent<PolygonCollider2D>();
            newCollider.usedByComposite = true;
            if (colliderIsForCollision)
                newCollider.isTrigger = true;
        }
    }

    public void SetPosition() //uses mins and maxes from CalcPos function to find random number between them; sets position to this
    {
        float randXPos = Random.Range(xPosLo, xPosHi);
        float randYPos = Random.Range(yPosLo, yPosHi);

        transform.localPosition = new Vector3(randXPos, randYPos, 1);
    }

    public void SetRotation() //uses mins and maxes from CalcRot function to find random number between them; sets rotation to this
    {
        float randRotate = Random.Range(rotateLo, rotateHi);

        transform.localEulerAngles = new Vector3(0, 0, randRotate);
    }

    public void SetScale() //uses mins and maxes from CalcScale function to find random number between them; sets scale to this. Also flips on X, if applicable
    {
        float rand = Random.value;
        int randLeftRight = 1; //so that if not allowed to flip on X, it will stay facing the default direction

        if (allowXFlipping == true)
        {
            if (rand > .5)
                randLeftRight = 1;

            else if (rand <= .5)
                randLeftRight = -1;
        }

        float randScale = Random.Range(scaleLo, scaleHi); //if you overallScaleMultiplier = .1, yields something between .9 and 1.1.

        float randWidth = Random.Range(widthLo, widthHi); 
        float randHeight = Random.Range(heightLo, heightHi); 

        transform.localScale = new Vector3(randLeftRight * (randWidth * randScale), (randHeight * randScale), 1); 
    }

    void TellScaleToStatsAndWander()
    {
        /*
        // if this script is on a GO, it's the only thing that alters its scale. Therefore it is the one that
        /// reports this altered scale to the 3 scripts that need it.
        
        Stats stats = GetComponent<Stats>();
        if (stats)
            stats.myScale = transform.localScale;

        CritterWanderAI critterWander = GetComponent<CritterWanderAI>();
        if (critterWander)
            critterWander.myScale = transform.localScale;

        FlyingWanderAI flyingWander = GetComponent<FlyingWanderAI>();
        if (flyingWander)
            flyingWander.myScale = transform.localScale;
            */
    }

}
