using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(TransformVariance))] //because otherwise no one sets stats.myScale
public class PeasantStats : Stats
{
    [Header("Sickness")]
    public Condition currentCondition;
    public enum Condition { Healthy, Stomachache, Headcold, Level3 }

    public GameObject stomachacheDetail, HeadcoldDetail;

    [Space(10)]
    public int currentWaitTime;

    public Transform potionCarrySpot; //drag in inspector
    public GameObject currentlyCarriedPotion;
    public PotionStats currentlyCarriedPotionStats;

    void Awake()
    {
        StatsAwakeStuff();
        selectable.OnASelected.AddListener(OnSelectableSelected);
    }

    void Start()
    {
        ChooseSickness();

        currentWaitTime = dayInfo.peasantWaitBeforeLeaveTime;
        StartCoroutine("CountdownToLeaving");
    }

    void ChooseSickness()
    {
        SetCondition(Condition.Healthy);

        float rand = Random.Range(0f, 1f);

        if (dayInfo.nightCount > 2)
        {
            SetCondition(Condition.Level3); //todo - no chance to reach headcold?

        }

        else if (dayInfo.nightCount > 1)
            SetCondition(Condition.Headcold);

        else
            SetCondition(Condition.Stomachache);
    }

    public void OnSelectableSelected()
    {
        UpdateSelectionMenu();
    }

    public void UpdateSelectionMenu()
    {
        selectionMenu.DeactivateAllButtonGOs();

        if (currentCondition != Condition.Healthy && suppliesCount.numPotions >= 1) 
            selectionMenu.PopulateButton(0, "GIVE POTION", delegate { StartCoroutine("GivePotion"); }, "GivePotion", this);

        else if (currentCondition == Condition.Healthy) 
            selectionMenu.PopulateButton(0, "CHAT", delegate { StartCoroutine("Chat"); }, "Chat", this);

        else 
            selectionMenu.actButtButt[0].interactable = false; //todo - not sure about this - only deactivates first button (from previous thing interacted with)? but should deactivate all?
        
    }

    public IEnumerator Chat() {
        selectionMenu.actButtButt[0].interactable = false;  //todo - not sure about this - only deactivates first button (from previous thing interacted with)? but should deactivate all?
        selectionManager.DeselectIt(selectable);

        StopCoroutine("CountdownToLeaving");
        moveAlong.StopMoveCoroutine();

        yield return new WaitForSeconds(1); 
    }

    public IEnumerator GivePotion()
    {
        selectionMenu.actButtButt[0].interactable = false;
        selectionManager.DeselectIt(selectable);

        StopCoroutine("CountdownToLeaving");
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

        SetCondition(Condition.Healthy);
        ChangeReputation(.05f);
        moveAlong.StartMoveCoroutineToVillage();


        /* todo should really switch the potion to the other arm since they turned around at this point, then do the following...
        SortingGroup potSortGroup = currentlyCarriedPotion.GetComponent<SortingGroup>();
        if (potSortGroup)
            potSortGroup.sortingLayerName = "Main"; */
    }

    IEnumerator CountdownToLeaving()
    {
        while (currentWaitTime > 0)
        {
            yield return new WaitForSeconds(1f);
            currentWaitTime--;
        }

        //if we reach this point, the witch has failed to serve the peasant in time - send the back to village and lose points.
        ChangeReputation(-.05f);
        moveAlong.StartMoveCoroutineToVillage();
        yield break; //exits coroutine 
    }

    void ReceivePotion()
    {
        suppliesCount.SubtractPotion();
        playerStats.currentlyCarriedItem.transform.SetParent(potionCarrySpot, false);
        currentlyCarriedPotion = playerStats.currentlyCarriedItem;
        playerStats.currentlyCarriedItem = null;
        currentlyCarriedPotion.transform.localPosition = new Vector3(0, 0, 0);
        currentlyCarriedPotionStats = currentlyCarriedPotion.GetComponent<PotionStats>();
    }

    void SetCondition(Condition newCondition)
    {
        if (newCondition == Condition.Healthy)
        {
            anim.SetTrigger("Healthy");
            stomachacheDetail.SetActive(false);
            HeadcoldDetail.SetActive(false);
            currentCondition = Condition.Healthy;
        }
        else if (newCondition == Condition.Stomachache)
        {
            anim.SetTrigger("Stomachache");
            stomachacheDetail.SetActive(true);
            HeadcoldDetail.SetActive(false);
            currentCondition = Condition.Stomachache;
        }
        else if (newCondition == Condition.Headcold)
        {
            anim.SetTrigger("Headcold");
            HeadcoldDetail.SetActive(true);
            stomachacheDetail.SetActive(false);
            currentCondition = Condition.Headcold;
        }
        else
        {
            print("no condition set or no paramaters for this condition");
        }
    }

    void OnDestroy()
    {
        selectable.OnASelected.RemoveListener(OnSelectableSelected);
    }
}
