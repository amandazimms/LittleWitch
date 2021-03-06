using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Events;

public class PlayerStats : Stats
{
    public CameraControls cameraControls;
    public Transform cameraTargetOutside;
    public Transform cameraTargetInside;

    public Transform carrySpot; //drag in inspector
    public GameObject currentlyCarriedItem;

    public GameObject potionPrefabSaltwater, potionPrefabBurning, potionPrefabEarthy, potionPrefabWindblown;
    public GameObject emptyBottlePrefab;
    public GameObject plantPrefab;
    //public CarriedFoodStats currentCarriedFoodStats;

    public GameObject poof;
    public Vector3 poofPosOffset;
    [HideInInspector] public SortingGroup mySG;
    public string sortingGroupOutsideHut;
    public string sortingGroupInsideHut;
    public GameObject mover;

    [HideInInspector] public DepthSorting depthSorting;

    public GameObject armForSpriteHiding;

    public bool isInsideHut;

    void Awake()
    {
        StatsAwakeStuff();
        selectable.OnASelected.AddListener(OnSelectableSelected);

        transform.localScale = myScale;
        playerMovement.myScale = myScale;
        mySG = GetComponent<SortingGroup>();

        cameraControls = Camera.main.GetComponent<CameraControls>();
        depthSorting = GetComponent<DepthSorting>();
    }

    void Start()
    {
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            print("========CLEAR===============================");
        }
    }

    public void OnSelectableSelected()
    {
        UpdateSelectionMenu();
    }

    public void UpdateSelectionMenu()
    {
        selectionMenu.DeactivateAllButtonGOs();

        //if we add any interacts here, do like berriesStats et al where  if (playerStats.isInBurrow) { selectionMenu.actButtButt[0].interactable = false; }
    }

    
    public void AppearInHand(GameObject gameObjectToAppear, bool instantiateNew)
    {
        //todo - first check for a currently carried item (shouldn't be any). If so, call the "put current item away" anim where player puts it back in bag before proceeding.
        if (instantiateNew)

            currentlyCarriedItem = Instantiate(gameObjectToAppear, carrySpot.position, Quaternion.Euler(0, 0, 0), carrySpot); // Quaternion.Euler(0f, 0f, 20f), carrySpot);
        else
        {
            gameObjectToAppear.transform.SetParent(carrySpot, false);
            currentlyCarriedItem = gameObjectToAppear;
        }
    }

    public void AppearInHandPotionCauldron(GameObject gameObjectToAppear, bool instantiateNew)
    {
        //todo - first check for a currently carried item (shouldn't be any). If so, call the "put current item away" anim where player puts it back in bag before proceeding.
        if (instantiateNew)

            currentlyCarriedItem = Instantiate(gameObjectToAppear, carrySpot.position, Quaternion.Euler(0f, 0f, 20f), carrySpot);
        else
        {
            gameObjectToAppear.transform.SetParent(carrySpot, false);
            currentlyCarriedItem = gameObjectToAppear;
        }
        PotionMaskVisibility potionMask = currentlyCarriedItem.GetComponent<PotionMaskVisibility>();
        potionMask.SetToCauldronVisibility();
    }

    public void SetVisiblityOutsideMask(bool isVisibleOutsideMask, GameObject parentGOofSprites)
    {
        SpriteRenderer[] sprites = parentGOofSprites.GetComponentsInChildren<SpriteRenderer>();

        if (isVisibleOutsideMask)
            foreach (SpriteRenderer sprite in sprites)
                sprite.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;

        else
            foreach (SpriteRenderer sprite in sprites)
                sprite.maskInteraction = SpriteMaskInteraction.None;
    }

    public void DoPoof()
    {
        GameObject thisPoof = Instantiate(poof, transform.position + poofPosOffset, Quaternion.identity);
        Destroy(thisPoof, .66f);
    }

    public void HideOrShow(bool isHiding)
    {
        if (isHiding)
            mover.SetActive(false);
        if (!isHiding)
            mover.SetActive(true);
    }

    public void SwitchToInsideHut()
    {
        isInsideHut = true;
        mySG.sortingLayerName = sortingGroupInsideHut;
        cameraControls.target = cameraTargetInside;

    }

    public void SwitchToOutsideHut()
    {
        isInsideHut = false;
        mySG.sortingLayerName = sortingGroupOutsideHut;
        cameraControls.target = cameraTargetOutside;
    }


    void OnDestroy()
    {
        selectable.OnASelected.RemoveListener(OnSelectableSelected);
    }


    //this magical bb copies and pastes a component at runtime. TODO put it in its own class?
    Component CopyComponent(Component original, GameObject destination)
    {
        System.Type type = original.GetType();
        Component copy = destination.AddComponent(type);

        // Copied fields can be restricted with BindingFlags
        System.Reflection.FieldInfo[] fields = type.GetFields();

        foreach (System.Reflection.FieldInfo field in fields)
            field.SetValue(copy, field.GetValue(original));

        return copy;
    }
}
