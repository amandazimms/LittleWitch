using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TransformVariance))] //because otherwise no one sets stats.myScale
public class PeasantStats : Stats
{
    public Transform potionCarrySpot; //drag in inspector
    public GameObject currentlyCarriedPotion;
    public PotionStats currentlyCarriedPotionStats;

    void Awake()
    {
        StatsAwakeStuff();
        selectable.OnASelected.AddListener(OnSelectableSelected);
    }

    public void OnSelectableSelected()
    {
        UpdateSelectionMenu();
    }

    public void UpdateSelectionMenu()
    {
        selectionMenu.DeactivateAllButtonGOs();

        if (playerStats.numPotions >= 1)
            selectionMenu.PopulateButton(0, "GIVE POTION", delegate { StartCoroutine("GivePotion"); }, "GivePotion", this);

        if (false)
            selectionMenu.actButtButt[0].interactable = false;
    }

    public IEnumerator GivePotion()
    {
        selectionMenu.actButtButt[0].interactable = false;
        selectionManager.DeselectIt(selectable);

        Vector3 playerTarget = CalculatePlayerTarget(transform, routeToOffset);
        playerMovement.MoveToObject(playerTarget, gameObject);

        StartCoroutine(WaitWhilePlayerEnrouteToMe(true, true));  //(false, false));

        waitForRoute = true; while (waitForRoute) { yield return null; }

        StartCoroutine(playerStats.FreezeFromMoving(true, 4, "GivePotion"));

        StartCoroutine(Anim(false, null, true, true, "GivePotion", 0, 0)); //player's anim starts

        playerStats.hasStartedAnimReachedKeyMoment = false; //player digging in bag
        while(!playerStats.hasStartedAnimReachedKeyMoment) { yield return null; }

        playerStats.PotionAppearInHand(); //potion appears

        yield return new WaitForSeconds(.41f); //player closing bag

        StartCoroutine(Anim(true, "ReceivePotion", false, false, null, 0, 0)); 

        hasStartedAnimReachedKeyMoment = false; 
        while (!hasStartedAnimReachedKeyMoment) { yield return null; }//both characters have hands outstretched

        ReceivePotion();

        hasStartedAnimReachedKeyMoment = false;
        while (!hasStartedAnimReachedKeyMoment) { yield return null; }//peasant ready to uncork

        currentlyCarriedPotionStats.Uncork();

        hasStartedAnimReachedKeyMoment = false;
        while (!hasStartedAnimReachedKeyMoment) { yield return null; }//potion is upright

        StartCoroutine(currentlyCarriedPotionStats.ChangeLiquidSpriteRendererAndMaskOverSeconds(.6f));//# seconds should be the time the peasant is holding the bottle upright in its anim

        hasStartedAnimFinished = false; while (!hasStartedAnimFinished) { yield return null; }
        playerStats.UnfreezeFromMoving(true, "GivePotion");

        moveAlong.StartMoveCoroutineToVillage();
    }

    void ReceivePotion()
    {
        playerStats.currentlyCarriedPotion.transform.SetParent(potionCarrySpot, false);
        currentlyCarriedPotion = playerStats.currentlyCarriedPotion;
        playerStats.currentlyCarriedPotion = null;
        currentlyCarriedPotion.transform.localPosition = new Vector3(0, 0, 0);
        currentlyCarriedPotionStats = currentlyCarriedPotion.GetComponent<PotionStats>();
    }



    void OnDestroy()
    {
        selectable.OnASelected.RemoveListener(OnSelectableSelected);
    }
}
