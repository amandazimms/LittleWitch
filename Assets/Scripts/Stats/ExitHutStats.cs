using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitHutStats : Stats
{
    public Transform apparitionPointB; //after arriving at gameObject, player poofs to this point on the ground

    public HutSwitcher hutSwitcher;

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

        if (playerStats.isInsideHut)
            selectionMenu.PopulateButton(0, "GO OUT", delegate { StartCoroutine("ExitHut"); }, "ExitHut", this);

        if (false)
            selectionMenu.actButtButt[0].interactable = false;
    }

    public IEnumerator ExitHut()
    {
        selectionMenu.actButtButt[0].interactable = false;
        selectionManager.DeselectIt(selectable);

        Vector3 playerTarget = CalculatePlayerTarget(transform, routeToOffset);
        playerMovement.MoveToObject(playerTarget, gameObject);

        StartCoroutine(WaitWhilePlayerEnrouteToMe(false, false));

        waitForRoute = true; while (waitForRoute) { yield return null; }

        StartCoroutine(playerStats.FreezeFromMoving(true, 3, "Apparate"));

        playerStats.depthSorting.enabled = false;
        playerStats.mySG.sortingOrder = 80;
        playerStats.SwitchToOutsideHut();
        hutSwitcher.SwitchToOutside();
        yield return new WaitForSeconds(.5f);


        StartCoroutine(Anim(false, null, true, true, "Apparate", 0, 0));

        playerStats.hasStartedAnimReachedKeyMoment = false; while (!playerStats.hasStartedAnimReachedKeyMoment) { yield return null; } //ready for poof!

        playerStats.DoPoof();

        playerStats.hasStartedAnimReachedKeyMoment = false; while (!playerStats.hasStartedAnimReachedKeyMoment) { yield return null; } //poof is completely covering player, ready for apparate

        playerStats.HideOrShow(true);
        playerStats.transform.position = apparitionPointB.position;
        playerStats.depthSorting.enabled = true;
        playerStats.DoPoof();

        yield return new WaitForSeconds(.12f);
        playerStats.HideOrShow(false);

        playerStats.hasStartedAnimFinished = false; while (!playerStats.hasStartedAnimFinished) { yield return null; }

        yield return null;
    }
}
