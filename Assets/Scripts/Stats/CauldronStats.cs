using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CauldronStats : Stats
{
    public enum BrewStage { Empty, Brewing, Done };
    public BrewStage currentBrewStage;
    public int maxPotionsPerBrew = 5;
    public int currentNumPotionsInCauldron;
    public int brewTime;

    public HourglassSpinner myHourglass;

    public Transform liquidLevel;
    public float[] liquidYpositions;
    public Color waterColorLiquid;
    public Color waterColorBubbles;
    public Color potionColorLiquid;
    public Color potionColorBubbles;

    public Transform ingredientsAppearSpot;
    public Transform ladleSpot;
    public GameObject currentIngredients;
    public GameObject myLadle;

    public SpriteRenderer liquidSR;
    public GameObject bubbles;
    SpriteRenderer[] bubblesSRs;

    public Transform interactPointBrew;
    public Transform interactPointBottle;

    bool becomeUnselectableAfterFillingAllPotions = false;

    float totalFadeSeconds = 8f;

    void Awake()
    {
        StatsAwakeStuff();
        selectable.OnASelected.AddListener(OnSelectableSelected);

        suppliesCount.OnPlantCountZero.AddListener(OnSuppliesPlantCountZero);
        suppliesCount.OnPlantCountPositive.AddListener(OnSuppliesPlantCountPositive);

        bubblesSRs = bubbles.GetComponentsInChildren<SpriteRenderer>();

        if (currentBrewStage == BrewStage.Brewing)
            currentBrewStage = BrewStage.Done;

        SetBrewStage(currentBrewStage);

        if (currentBrewStage == BrewStage.Done)
        {
            liquidSR.color = potionColorLiquid;
            foreach (SpriteRenderer sprite in bubblesSRs)
            sprite.color = potionColorBubbles;
        }

        if (brewTime == 0)
            brewTime = 1;
    }

    public void OnSelectableSelected()
    {
        UpdateSelectionMenu();
    }

    public void UpdateSelectionMenu()
    {
        selectionMenu.DeactivateAllButtonGOs();

        //todo need to specify which potion
        if (currentBrewStage == BrewStage.Empty && suppliesCount.numPlants > 0)
            selectionMenu.PopulateButton(0, "BREW POTION", delegate { StartCoroutine("BrewPotion"); }, "BrewPotion", this);

        //SALTWATER - need to copy paste for other cases
        else if (currentBrewStage == BrewStage.Done && currentNumPotionsInCauldron > 0) {
            selectionMenu.PopulateButton(0, "BOTTLE A POTION", delegate { StartCoroutine( BottlePotion(playerStats.potionPrefabSaltwater) ); }, "BottlePotion", this);
            selectionMenu.PopulateButton(1, "BOTTLE ALL POTIONS", delegate { StartCoroutine( BottleAllPotions(playerStats.potionPrefabSaltwater) ); }, "BottleAllPotions", this);
        }

        else if (currentBrewStage == BrewStage.Brewing)
            selectionMenu.actButtButt[0].interactable = false;
    }

    public IEnumerator BrewPotion()
    {
        selectionMenu.actButtButt[0].interactable = false;
        selectionManager.DeselectIt(selectable);

        Vector3 playerTarget = CalculatePlayerTargetExact(interactPointBrew);
        playerMovement.MoveToObject(playerTarget, gameObject);

        StartCoroutine(WaitWhilePlayerEnrouteToMe(false, false)); 
        waitForRoute = true; while (waitForRoute) { yield return null; }

        StartCoroutine(playerStats.FreezeFromMoving(true, 10, "BrewPotion"));
        
        StartCoroutine(Anim(true, "BrewPotion", true, true, "BrewPotion", 0, 0));
        StartCoroutine(RaiseLiquidToTop(1.33f));

        playerStats.hasStartedAnimReachedKeyMoment = false; //player digging in bag
        while (!playerStats.hasStartedAnimReachedKeyMoment) { yield return null; }

        playerStats.AppearInHand(playerStats.plantPrefab, true); //plants appear in hand

        playerStats.hasStartedAnimReachedKeyMoment = false; //plants airborne
        while (!playerStats.hasStartedAnimReachedKeyMoment) { yield return null; }

        Destroy(playerStats.currentlyCarriedItem);
        playerStats.currentlyCarriedItem = null;

        playerStats.hasStartedAnimReachedKeyMoment = false; //player stirring, color ready to change
        while (!playerStats.hasStartedAnimReachedKeyMoment) { yield return null; }

        foreach (SpriteRenderer sprite in bubblesSRs)
            StartCoroutine(ChangeLiquidColor(sprite, potionColorBubbles));
        StartCoroutine(ChangeLiquidColor(liquidSR, potionColorLiquid));

        playerStats.hasStartedAnimFinished = false; while (!playerStats.hasStartedAnimFinished) { yield return null; }

        StartCoroutine(myHourglass.Rotate(1f / brewTime));
        

        //todo change to brewing
        SetBrewStage(BrewStage.Brewing);
        suppliesCount.SubtractPlant();
        playerStats.UnfreezeFromMoving(true, "BrewPotion"); //doing this here to speed up gameplay and make it possible to dose up a lot of peasants at once
    }

    public IEnumerator BottlePotion(GameObject potionToBottle)
    {
        selectionMenu.actButtButt[0].interactable = false;
        selectionManager.DeselectIt(selectable);

        Vector3 playerTarget = CalculatePlayerTargetExact(interactPointBottle);
        playerMovement.MoveToObject(playerTarget, gameObject);

        StartCoroutine(WaitWhilePlayerEnrouteToMe(false, false));
        waitForRoute = true; while (waitForRoute) { yield return null; }

        StartCoroutine(playerStats.FreezeFromMoving(true, 7, "BottlePotion"));

        StartCoroutine(Anim(false, null, true, true, "BottlePotion", 0, 0));

        playerStats.hasStartedAnimReachedKeyMoment = false; //player digging in bag
        while (!playerStats.hasStartedAnimReachedKeyMoment) { yield return null; }

        playerStats.AppearInHand(potionToBottle, true); //empty bottle appear in hand
        PotionStats potionStats = playerStats.currentlyCarriedItem.GetComponent<PotionStats>();
        StartCoroutine(potionStats.FillAnim(.6f));
        //note that Potion.Fill animation lines up with this; starting with dead time. So nothing to activate or synchronize there.

        yield return new WaitForSeconds(1.08f);
        StartCoroutine(LowerLiquidLevel1(.6f));

        playerStats.hasStartedAnimReachedKeyMoment = false; //about to put potion away
        while (!playerStats.hasStartedAnimReachedKeyMoment) { yield return null; }

        Destroy(playerStats.currentlyCarriedItem);
        playerStats.currentlyCarriedItem = null;
        suppliesCount.AddPotion();

        playerStats.hasStartedAnimFinished = false; while (!playerStats.hasStartedAnimFinished) { yield return null; }

        currentNumPotionsInCauldron--;
        if (currentNumPotionsInCauldron == 0)
        {
            SetBrewStage(BrewStage.Empty);
            if (becomeUnselectableAfterFillingAllPotions)
            {
                selectable.isCurrentlyUnselectable = true;
                becomeUnselectableAfterFillingAllPotions = false;
            }
        }
        playerStats.UnfreezeFromMoving(true, "BottlePotion"); //doing this here to speed up gameplay and make it possible to dose up a lot of peasants at once
    }

    public IEnumerator BottleAllPotions(GameObject potionsToBottle)
    {
        selectionMenu.actButtButt[1].interactable = false;
        selectionManager.DeselectIt(selectable);

        Vector3 playerTarget = CalculatePlayerTargetExact(interactPointBottle);
        playerMovement.MoveToObject(playerTarget, gameObject);

        StartCoroutine(WaitWhilePlayerEnrouteToMe(false, false));
        waitForRoute = true; while (waitForRoute) { yield return null; }

        while (currentNumPotionsInCauldron > 0)
        {
            StartCoroutine(playerStats.FreezeFromMoving(true, 6, "BottlePotion"));

            playerStats.anim.speed = 2f;
            if (currentNumPotionsInCauldron == maxPotionsPerBrew)
                StartCoroutine(Anim(false, null, true, true, "BottlePotionLoopStart", 0, 0));
            else if (currentNumPotionsInCauldron == 1)
            {
                print("1 left");
                StartCoroutine(Anim(false, null, true, true, "BottlePotionLoopEnd", 0, 0));
            }
            else 
                StartCoroutine(Anim(false, null, true, true, "BottlePotionLoop", 0, 0));

            playerStats.hasStartedAnimReachedKeyMoment = false; //player digging in bag
            while (!playerStats.hasStartedAnimReachedKeyMoment) { yield return null; }

            playerStats.AppearInHand(potionsToBottle, true); //empty bottle appear in hand
            PotionStats potionStats = playerStats.currentlyCarriedItem.GetComponent<PotionStats>();
            StartCoroutine(potionStats.FillAnim(.3f));
            //note that Potion.Fill animation lines up with this; starting with dead time. So nothing to activate or synchronize there.

            yield return new WaitForSeconds(.54f);
            StartCoroutine(LowerLiquidLevel1(.3f));

            playerStats.hasStartedAnimReachedKeyMoment = false; //about to put potion away
            while (!playerStats.hasStartedAnimReachedKeyMoment) { yield return null; }

            Destroy(playerStats.currentlyCarriedItem);
            playerStats.currentlyCarriedItem = null;
            suppliesCount.AddPotion();

            playerStats.hasStartedAnimFinished = false; while (!playerStats.hasStartedAnimFinished) { yield return null; }

            playerStats.anim.speed = 1f;

            currentNumPotionsInCauldron--;
            if (currentNumPotionsInCauldron == 0)
            {
                SetBrewStage(BrewStage.Empty);
                if (becomeUnselectableAfterFillingAllPotions)
                {
                    selectable.isCurrentlyUnselectable = true;
                    becomeUnselectableAfterFillingAllPotions = false;
                }
            }
            playerStats.UnfreezeFromMoving(true, "BottlePotion"); //doing this here to speed up gameplay and make it possible to dose up a lot of peasants at once
        }
    }

    public void AppearAtSpot(GameObject gameObjectToAppear, Vector3 spotToAppearAt, bool instantiateNew)
    {
        if (instantiateNew)

            currentIngredients = Instantiate(gameObjectToAppear, ingredientsAppearSpot.position, Quaternion.identity, ingredientsAppearSpot);
        else
        {
            gameObjectToAppear.transform.SetParent(ingredientsAppearSpot, false);
            currentIngredients = gameObjectToAppear;
        }
    }

    void SetBrewStage(BrewStage _brewStage)
    {
        if (_brewStage == BrewStage.Brewing)
        {
            selectable.isCurrentlyUnselectable = true;
            currentBrewStage = BrewStage.Brewing;
            StartCoroutine(BrewCountdown());
        }

        if (_brewStage == BrewStage.Empty)
        {
            currentBrewStage = BrewStage.Empty;
            liquidLevel.localPosition = new Vector3(liquidLevel.localPosition.x, liquidYpositions[0], liquidLevel.localPosition.z);
            currentNumPotionsInCauldron = 0;
            liquidSR.color = waterColorLiquid;
            foreach (SpriteRenderer sprite in bubblesSRs)
                sprite.color = waterColorBubbles;
        }

        if (_brewStage == BrewStage.Done)
        {
            selectable.isCurrentlyUnselectable = false;
            currentBrewStage = BrewStage.Done;
            liquidLevel.localPosition = new Vector3(liquidLevel.localPosition.x, liquidYpositions[5], liquidLevel.localPosition.z);
            currentNumPotionsInCauldron = maxPotionsPerBrew;
        }
    }


    IEnumerator BrewCountdown()
    {
        int currentBrewCountdown = brewTime;

        while (currentBrewCountdown > 0)
        {
            currentBrewCountdown--;
            yield return new WaitForSeconds(1f);
        }

        SetBrewStage(BrewStage.Done);
    }

    IEnumerator ChangeLiquidColor (SpriteRenderer sprite, Color color)
    {
        Color originalColor = sprite.color;
        float lerp = 0;
        float currentFadeSeconds = 0f;


        while (currentFadeSeconds < totalFadeSeconds)
        {
            currentFadeSeconds += Time.deltaTime;
            lerp = currentFadeSeconds / totalFadeSeconds;

            sprite.color = Color.Lerp(originalColor, color, lerp);
            yield return null;
        }
    }

    IEnumerator LowerLiquidLevel1(float totalSeconds)
    {
        float originalYpos = liquidLevel.localPosition.y;
        float endYPos = liquidYpositions[currentNumPotionsInCauldron - 1];

        float lerp = 0;
        float currentSeconds = 0f;

        while (currentSeconds < totalSeconds)
        {
            currentSeconds += Time.deltaTime;
            lerp = currentSeconds / totalSeconds;

            float y = Mathf.Lerp(originalYpos, endYPos, lerp);
            liquidLevel.localPosition = new Vector3(liquidLevel.localPosition.x, y, liquidLevel.localPosition.z);

            yield return null;
        }
    }

    IEnumerator RaiseLiquidToTop(float totalSeconds)
    {
        float originalYpos = liquidLevel.localPosition.y;
        float endYPos = liquidYpositions[5];

        float lerp = 0;
        float currentSeconds = 0f;

        while (currentSeconds < totalSeconds)
        {
            currentSeconds += Time.deltaTime;
            lerp = currentSeconds / totalSeconds;

            float y = Mathf.Lerp(originalYpos, endYPos, lerp);
            liquidLevel.localPosition = new Vector3(liquidLevel.localPosition.x, y, liquidLevel.localPosition.z);

            yield return null;
        }
    }

    void OnSuppliesPlantCountZero()
    {
        becomeUnselectableAfterFillingAllPotions = true;
    }

    void OnSuppliesPlantCountPositive()
    {
        selectable.isCurrentlyUnselectable = false;
    }
}
