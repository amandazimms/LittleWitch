using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CauldronStats : Stats
{
    public enum BrewStage { Empty, Brewing, Done };
    public BrewStage currentBrewStage;
    public int maxPotionsPerBrew = 5;
    public int currentNumPotionsInCauldron;

    public Transform liquid;
    public float[] liquidYpositions;

    public Transform ingredientsAppearSpot;
    public Transform ladleSpot;
    public GameObject currentIngredients;
    public GameObject myLadle;

    public SpriteRenderer liquidSR;
    public GameObject bubbles;
    SpriteRenderer[] bubblesSRs;

    public Transform interactPointBrew;
    public Transform interactPointBottle;

    float totalFadeSeconds = 8f;

    void Awake()
    {
        StatsAwakeStuff();
        selectable.OnASelected.AddListener(OnSelectableSelected);

        bubblesSRs = bubbles.GetComponentsInChildren<SpriteRenderer>();
    }

    public void OnSelectableSelected()
    {
        UpdateSelectionMenu();
    }

    public void UpdateSelectionMenu()
    {
        selectionMenu.DeactivateAllButtonGOs();

        if (currentBrewStage == BrewStage.Empty && playerStats.numPlants > 0)
            selectionMenu.PopulateButton(0, "BREW POTION", delegate { StartCoroutine("BrewPotion"); }, "BrewPotion", this);

        if (currentBrewStage == BrewStage.Done && currentNumPotionsInCauldron > 0)
            selectionMenu.PopulateButton(0, "BOTTLE POTION", delegate { StartCoroutine("BottlePotion"); }, "BottlePotion", this);

        if (false)
            selectionMenu.actButtButt[0].interactable = false;
    }

    public IEnumerator BrewPotion()
    {
        selectionMenu.actButtButt[0].interactable = false;
        selectionManager.DeselectIt(selectable);

        //Vector3 playerTarget = CalculatePlayerTarget(transform, routeToOffset);
        Vector3 playerTarget = CalculatePlayerTargetExact(interactPointBrew);
        playerMovement.MoveToObject(playerTarget, gameObject);

        StartCoroutine(WaitWhilePlayerEnrouteToMe(false, false)); 
        waitForRoute = true; while (waitForRoute) { yield return null; }

        StartCoroutine(playerStats.FreezeFromMoving(true, 10, "BrewPotion"));
        
        StartCoroutine(Anim(true, "BrewPotion", true, true, "BrewPotion", 0, 0));

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
            StartCoroutine(ChangeLiquidColor(sprite, new Color(0.8120158f, 1f, 0f)));
        StartCoroutine(ChangeLiquidColor(liquidSR, new Color(0.04839636f, 0.7830189f, 0f)));

        playerStats.hasStartedAnimFinished = false; while (!playerStats.hasStartedAnimFinished) { yield return null; }

        //todo change to brewing 
        currentBrewStage = BrewStage.Done;
        currentNumPotionsInCauldron = maxPotionsPerBrew;
        playerStats.numPlants--;
        playerStats.UnfreezeFromMoving(true, "BrewPotion"); //doing this here to speed up gameplay and make it possible to dose up a lot of peasants at once
    }

    public IEnumerator BottlePotion()
    {
        selectionMenu.actButtButt[0].interactable = false;
        selectionManager.DeselectIt(selectable);

        Vector3 playerTarget = CalculatePlayerTargetExact(interactPointBottle);
        playerMovement.MoveToObject(playerTarget, gameObject);

        StartCoroutine(WaitWhilePlayerEnrouteToMe(false, false));
        waitForRoute = true; while (waitForRoute) { yield return null; }

        StartCoroutine(playerStats.FreezeFromMoving(true, 6, "BottlePotion"));

        StartCoroutine(Anim(false, null, true, true, "BottlePotion", 0, 0));

        playerStats.hasStartedAnimReachedKeyMoment = false; //player digging in bag
        while (!playerStats.hasStartedAnimReachedKeyMoment) { yield return null; }

        playerStats.AppearInHand(playerStats.potionPrefab, true); //empty bottle appear in hand
        PotionStats potionStats = playerStats.currentlyCarriedItem.GetComponent<PotionStats>();
        StartCoroutine(potionStats.FillAnim(.6f));
        //note that Potion.Fill animation lines up with this; starting with dead time. So nothing to activate or synchronize there.

        yield return new WaitForSeconds(1.08f);
        StartCoroutine(ChangeLiquidLevel(.6f));

        playerStats.hasStartedAnimReachedKeyMoment = false; //about to put potion away
        while (!playerStats.hasStartedAnimReachedKeyMoment) { yield return null; }

        Destroy(playerStats.currentlyCarriedItem);
        playerStats.currentlyCarriedItem = null;
        playerStats.numPotions++;

        playerStats.hasStartedAnimFinished = false; while (!playerStats.hasStartedAnimFinished) { yield return null; }

        currentNumPotionsInCauldron--;
        if (currentNumPotionsInCauldron == 0)
            currentBrewStage = BrewStage.Empty;
        playerStats.UnfreezeFromMoving(true, "BottlePotion"); //doing this here to speed up gameplay and make it possible to dose up a lot of peasants at once
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

    IEnumerator ChangeLiquidLevel(float totalSeconds)
    {
        float originalYpos = liquid.localPosition.y;
        float endYPos = liquidYpositions[currentNumPotionsInCauldron - 1];

        float lerp = 0;
        float currentSeconds = 0f;

        while (currentSeconds < totalSeconds)
        {
            currentSeconds += Time.deltaTime;
            lerp = currentSeconds / totalFadeSeconds;

            float y = Mathf.Lerp(originalYpos, endYPos, lerp);
            liquid.localPosition = new Vector3(liquid.localPosition.x, y, liquid.localPosition.z);

            yield return null;
        }
    }
}
