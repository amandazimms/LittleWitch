using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(TransformVariance))] //because otherwise no one sets stats.myScale
public class PeasantStats : Stats
{
    public bool isSick;

    public int maxWaitTime = 20;
    public int currentWaitTime;

    public SpriteRenderer faceTint;
    public Color faceTintColorSick;
    public Color faceTintColorNormal;

    public Transform potionCarrySpot; //drag in inspector
    public GameObject currentlyCarriedPotion;
    public PotionStats currentlyCarriedPotionStats;

    void Awake()
    {
        StatsAwakeStuff();
        selectable.OnASelected.AddListener(OnSelectableSelected);
    }

    private void Start()
    {
        anim.SetTrigger("Stomachache");
        isSick = true;
        faceTint.color = faceTintColorSick;

        currentWaitTime = maxWaitTime; 
        StartCoroutine("WaitAtHut");
    }

    public void OnSelectableSelected()
    {
        UpdateSelectionMenu();
    }

    public void UpdateSelectionMenu()
    {
        selectionMenu.DeactivateAllButtonGOs();

        if (isSick && playerStats.numPotions >= 1)
            selectionMenu.PopulateButton(0, "GIVE POTION", delegate { StartCoroutine("GivePotion"); }, "GivePotion", this);

        if (false)
            selectionMenu.actButtButt[0].interactable = false;
    }

    public IEnumerator GivePotion()
    {
        selectionMenu.actButtButt[0].interactable = false;
        selectionManager.DeselectIt(selectable);

        StopCoroutine("WaitAtHut");
        moveAlong.StopMoveCoroutine();

        Vector3 playerTarget = CalculatePlayerTarget(transform, routeToOffset);
        playerMovement.MoveToObject(playerTarget, gameObject);

        StartCoroutine(WaitWhilePlayerEnrouteToMe(false, true)); //since we froze the peasant already, it's not considered a moving target

        waitForRoute = true; while (waitForRoute) { yield return null; }

        StartCoroutine(playerStats.FreezeFromMoving(true, 4, "GivePotion"));

        anim.SetTrigger("Healthy");
        StartCoroutine(Anim(false, null, true, true, "GivePotion", 0, 0)); //player's anim starts

        playerStats.hasStartedAnimReachedKeyMoment = false; //player digging in bag
        while(!playerStats.hasStartedAnimReachedKeyMoment) { yield return null; }

        playerStats.AppearInHand(playerStats.potionPrefab, true); //potion appears

        yield return new WaitForSeconds(.41f); //player closing bag

        StartCoroutine(Anim(true, "ReceivePotion", false, false, null, 0, 0)); 

        hasStartedAnimReachedKeyMoment = false; 
        while (!hasStartedAnimReachedKeyMoment) { yield return null; }//both characters have hands outstretched

        ReceivePotion();
        playerStats.UnfreezeFromMoving(true, "GivePotion"); //doing this here to speed up gameplay and make it possible to dose up a lot of peasants at once

        hasStartedAnimReachedKeyMoment = false;
        while (!hasStartedAnimReachedKeyMoment) { yield return null; }//peasant ready to uncork

        currentlyCarriedPotionStats.Uncork();

        hasStartedAnimReachedKeyMoment = false;
        while (!hasStartedAnimReachedKeyMoment) { yield return null; }//potion is upright

        StartCoroutine(currentlyCarriedPotionStats.DrinkAnim(.6f));//# seconds should be the time the peasant is holding the bottle upright in its anim

        hasStartedAnimFinished = false; while (!hasStartedAnimFinished) { yield return null; }

        CureSickness();
        moveAlong.StartMoveCoroutineToVillage();


        /* todo should really switch the potion to the other arm since they turned around at this point, then do the following...
        SortingGroup potSortGroup = currentlyCarriedPotion.GetComponent<SortingGroup>();
        if (potSortGroup)
            potSortGroup.sortingLayerName = "Main"; */
    }

    IEnumerator WaitAtHut()
    {
        yield return new WaitForSeconds(1f);
        currentWaitTime--;

        if (currentWaitTime <= 0) //time's up
        {
            ChangeReputation(-.05f);
            moveAlong.StartMoveCoroutineToVillage();
            yield break; //exits coroutine 
        }
        StartCoroutine("WaitAtHut");
    }

    void ReceivePotion()
    {
        playerStats.numPotions--;
        playerStats.currentlyCarriedItem.transform.SetParent(potionCarrySpot, false);
        currentlyCarriedPotion = playerStats.currentlyCarriedItem;
        playerStats.currentlyCarriedItem = null;
        currentlyCarriedPotion.transform.localPosition = new Vector3(0, 0, 0);
        currentlyCarriedPotionStats = currentlyCarriedPotion.GetComponent<PotionStats>();
    }

    void CureSickness()
    {
        isSick = false;
        faceTint.color = faceTintColorNormal;
        ChangeReputation(.05f);
    }


    void OnDestroy()
    {
        selectable.OnASelected.RemoveListener(OnSelectableSelected);
    }
}
