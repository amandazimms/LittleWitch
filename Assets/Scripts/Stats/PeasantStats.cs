using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(TransformVariance))] //because otherwise no one sets stats.myScale
public class PeasantStats : Stats
{
    [Header("Sickness")]
    public Condition currentCondition;
    public enum Condition { Healthy, FireBreath, AirHeaded, Soiled, TheDrips }

    public ParticleSystem airheadPartis, soiledPartis;
    public ParticleSystem[] firebreathPartis, thedripsPartis;

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
        //todo tie to difficulty rating / day index
        float rand = Random.Range(0f, 1f);

        if (rand <= .25)
            SetCondition(Condition.AirHeaded);

        else if (rand > .325 && rand <= .5)
            SetCondition(Condition.FireBreath);

        else if (rand > .5 && rand <= .75)
            SetCondition(Condition.Soiled);

        else 
            SetCondition(Condition.TheDrips);
    }

    public void OnSelectableSelected()
    {
        UpdateSelectionMenu();
    }

    public void UpdateSelectionMenu()
    {
        selectionMenu.DeactivateAllButtonGOs();

        //GIVE SALTWATER BREW
        if (currentCondition != Condition.Healthy && suppliesCount.numPotions >= 1) //todo - in later phases: if we have this kind of potion
            selectionMenu.PopulateButton(0, "GIVE SALTWATER BREW", delegate { StartCoroutine (GivePotion(playerStats.potionPrefabSaltwater) ); }, "GivePotion", this);

        //GIVE BOTTLED BURNING
        if (currentCondition != Condition.Healthy && suppliesCount.numPotions >= 1) //todo - in later phases: if we have this kind of potion
            selectionMenu.PopulateButton(1, "GIVE BOTTLED BURNING", delegate { StartCoroutine(GivePotion(playerStats.potionPrefabBurning)); }, "GivePotion", this);

        //GIVE EARTHY ELIXIR
        if (currentCondition != Condition.Healthy && suppliesCount.numPotions >= 1) //todo - in later phases: if we have this kind of potion
            selectionMenu.PopulateButton(2, "GIVE EARTHY ELIXIR", delegate { StartCoroutine(GivePotion(playerStats.potionPrefabEarthy)); }, "GivePotion", this);

        //GIVE WINDBLOWN TONIC
        if (currentCondition != Condition.Healthy && suppliesCount.numPotions >= 1) //todo - in later phases: if we have this kind of potion
            selectionMenu.PopulateButton(3, "GIVE WINDBLOWN TONIC", delegate { StartCoroutine(GivePotion(playerStats.potionPrefabWindblown)); }, "GivePotion", this);

        //else if (true) 
        //    selectionMenu.PopulateButton(0, "CHAT", delegate { StartCoroutine("Chat"); }, "Chat", this);

        else 
            selectionMenu.actButtButt[0].interactable = false; //todo - not sure about this - only deactivates first button (from previous thing interacted with)? but should deactivate all?
        
    }

    public IEnumerator Chat() {
        selectionMenu.actButtButt[0].interactable = false;  //todo - not sure about this - only deactivates first button (from previous thing interacted with)? but should deactivate all?
        selectionManager.DeselectIt(selectable);

        //todo this function doesn't do anything yet

        StopCoroutine("CountdownToLeaving");
        moveAlong.StopMoveCoroutine();

        yield return new WaitForSeconds(1); 
    }

    public IEnumerator GivePotion(GameObject potionTypeToGive)
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

        //anim.SetTrigger("Healthy");todo delete from here?
        StartCoroutine(Anim(false, null, true, true, "GivePotion", 0, 0)); //player's anim starts

        playerStats.hasStartedAnimReachedKeyMoment = false; //player digging in bag
        while(!playerStats.hasStartedAnimReachedKeyMoment) { yield return null; }

        playerStats.AppearInHand(potionTypeToGive, true); //potion appears

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
        //todo add feedback here - e.g. when drinking bottled burning, show fire FX around the peasant

        hasStartedAnimFinished = false; while (!hasStartedAnimFinished) { yield return null; }

        if (CureChecker(potionTypeToGive))
        {   //if the cure was successful
            //todo add feedback here - happy peasant, ui update
            SetCondition(Condition.Healthy);
            ChangeReputation(.05f);
            moveAlong.StartMoveCoroutineToVillage();
        } else
        {   //if the cure was NOT successful
            //todo add feedback here - mad peasant, ui update
            ChangeReputation(-.1f);
            moveAlong.StartMoveCoroutineToVillage();
        }
        /* todo should really switch the potion to the other arm since they turned around at this point, then do the following...
        SortingGroup potSortGroup = currentlyCarriedPotion.GetComponent<SortingGroup>();
        if (potSortGroup)
            potSortGroup.sortingLayerName = "Main"; */
    }

    public bool CureChecker(GameObject potionTypeGiven)
    {   //tests if the right potion was administered to the right sickness

        if (potionTypeGiven.name == "SaltwaterBrew" && currentCondition == Condition.FireBreath)
            return true;

        else if (potionTypeGiven.name == "BottledBurning" && currentCondition == Condition.AirHeaded)
            return true;

        else if (potionTypeGiven.name == "WindblownTonic" && currentCondition == Condition.Soiled)
            return true;

        else if (potionTypeGiven.name == "EarthyElixir" && currentCondition == Condition.TheDrips)
            return true;

        else
            return false;
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
            airheadPartis.Stop();
            foreach (ParticleSystem parti in thedripsPartis)
                parti.Stop();

            currentCondition = Condition.Healthy;
        }
        else if (newCondition == Condition.FireBreath)
        {
            anim.SetTrigger("FireBreath");
           
            currentCondition = Condition.FireBreath;
        }
        else if (newCondition == Condition.AirHeaded)
        {
            anim.SetTrigger("AirHeaded");
            airheadPartis.Play(); //since airhead particles are a looping ps, we initialize them here with play, and turn them off when condition = healthy.

            currentCondition = Condition.AirHeaded;
        }
        else if (newCondition == Condition.Soiled)
        {
            anim.SetTrigger("Soiled");
           
            currentCondition = Condition.Soiled;
        }
        else if (newCondition == Condition.TheDrips)
        {
            anim.SetTrigger("TheDrips");

            foreach (ParticleSystem parti in thedripsPartis)
                parti.Play();

            currentCondition = Condition.TheDrips;
        }
        else
        {
            print("no condition set or no paramaters for this condition");
        }
    }

    public void PlayFirebreathParticles() //triggered from animation event
    { //since firebreath particles don't loop, we set them to play here (once each time), and don't need to turn them off when condition = healthy
        foreach (ParticleSystem parti in firebreathPartis)
        {
            parti.Play();
        }


    }

    public void PlaySoiledParticles() //triggered from animation event
    { //since soiled particles don't loop, we set them to play here (once each time), and don't need to turn them off when condition = healthy
        soiledPartis.Play();
    }

    void OnDestroy()
    {
        selectable.OnASelected.RemoveListener(OnSelectableSelected);
    }
}
