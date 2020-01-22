using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantPotStats : Stats
{
    public PlantGrow[] plantGrows;

    public float individualPlantGrowTime;

    public Transform interactPointHarvest;

    public int numMaturePlants;

    void Awake()
    {
        StatsAwakeStuff();
        selectable.OnASelected.AddListener(OnSelectableSelected);

        if (individualPlantGrowTime == 0)
        {
            print("Zero!");
            individualPlantGrowTime = 1;
        }

        StartActivePlants();
    }

    public void OnSelectableSelected()
    {
        UpdateSelectionMenu();
    }

    public void UpdateSelectionMenu()
    {
        selectionMenu.DeactivateAllButtonGOs();

        if (numMaturePlants >= 1)
            selectionMenu.PopulateButton(0, "HARVEST", delegate { StartCoroutine("Harvest"); }, "Harvest", this);

        if (false)
            selectionMenu.actButtButt[0].interactable = false;
    }

    public IEnumerator Harvest()
    {
        selectionMenu.actButtButt[0].interactable = false;
        selectionManager.DeselectIt(selectable);

        Vector3 playerTarget = CalculatePlayerTargetExact(interactPointHarvest);
        playerMovement.MoveToObject(playerTarget, gameObject);

        StartCoroutine(WaitWhilePlayerEnrouteToMe(false, false));
        waitForRoute = true; while (waitForRoute) { yield return null; }

        StartCoroutine(playerStats.FreezeFromMoving(true, 2, "Harvest"));

        StartCoroutine(Anim(true, "Harvest", true, true, "Harvest", 0, 0));
        DoHarvestOnRandomMaturePlant();

        playerStats.hasStartedAnimReachedKeyMoment = false; //player pulling on leaf 
        while (!playerStats.hasStartedAnimReachedKeyMoment) { yield return null; }

        playerStats.AppearInHand(playerStats.plantPrefab, true); //gathered plants appear in hand

        playerStats.hasStartedAnimReachedKeyMoment = false; //player going toward bag 
        while (!playerStats.hasStartedAnimReachedKeyMoment) { yield return null; }

        Destroy(playerStats.currentlyCarriedItem);
        playerStats.currentlyCarriedItem = null;
        suppliesCount.AddPlant();

        playerStats.hasStartedAnimFinished = false; while (!playerStats.hasStartedAnimFinished) { yield return null; }

        playerStats.UnfreezeFromMoving(true, "Harvest"); 
    }

    void DoHarvestOnRandomMaturePlant()
    {
        PlantGrow thisMaturePlant = plantGrows[Random.Range(0, plantGrows.Length)];
        if (thisMaturePlant.currentGrowStage != PlantGrow.GrowStage.Mature)
            DoHarvestOnRandomMaturePlant();
        else
            thisMaturePlant.StartHarvest();
    }

    void StartActivePlants()
    {
        plantGrows = GetComponentsInChildren<PlantGrow>();

        for (int i = 0; i < plantGrows.Length; i++)
        {
            plantGrows[i].plantPot = this;
            plantGrows[i].growTime = individualPlantGrowTime;
            plantGrows[i].initialWaitToGrowTime = i * individualPlantGrowTime;
        }        
    }

    public void SubtractMaturePlant()
    {
        numMaturePlants--;
        if (numMaturePlants == 0)
            selectable.isCurrentlyUnselectable = true;
    }

    public void AddMaturePlant()
    {
        numMaturePlants++;
        if (selectable.isCurrentlyUnselectable)
            selectable.isCurrentlyUnselectable = false;
    }


}
