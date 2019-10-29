using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterHutStats : Stats
{
    //goes on the door (outside of hut)

    public Transform apparitionPointA; //player walks to this point, then poofs onto front stoop.
    public Transform apparitionPointB; //after arriving at point A, player poofs to this point at the stoop.
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

        if (!playerStats.isInsideHut)
            selectionMenu.PopulateButton(0, "GO IN", delegate { StartCoroutine("EnterHut"); }, "EnterHut", this);

        if (false)
            selectionMenu.actButtButt[0].interactable = false;
    }

    public IEnumerator EnterHut()
    {
        selectionMenu.actButtButt[0].interactable = false;
        selectionManager.DeselectIt(selectable);

        Vector3 playerTarget = CalculatePlayerTarget(apparitionPointA, routeToOffset);
        playerMovement.MoveToObject(playerTarget, gameObject);

        StartCoroutine(WaitWhilePlayerEnrouteToMe(false, false)); 

        waitForRoute = true; while (waitForRoute) { yield return null; }

        StartCoroutine(playerStats.FreezeFromMoving(true, 3, "Apparate"));

        StartCoroutine(Anim(false, null, true, true, "Apparate", 0, 0));

        playerStats.hasStartedAnimReachedKeyMoment = false; while (!playerStats.hasStartedAnimReachedKeyMoment) { yield return null; } //ready for poof!

        playerStats.DoPoof();

        playerStats.hasStartedAnimReachedKeyMoment = false; while (!playerStats.hasStartedAnimReachedKeyMoment) { yield return null; } //poof is completely covering player, ready for apparate

        playerStats.HideOrShow(true);
        playerStats.depthSorting.enabled = false;
        playerStats.transform.position = apparitionPointB.position;
        playerStats.DoPoof();

        yield return new WaitForSeconds(.12f);
        playerStats.HideOrShow(false);

        playerStats.hasStartedAnimFinished = false; while (!playerStats.hasStartedAnimFinished) { yield return null; }

        hutSwitcher.SwitchToInside();

        yield return new WaitForSeconds(hutSwitcher.totalFadeSeconds); 
        playerStats.SwitchToInsideHut();
        playerStats.depthSorting.enabled = true;
    }
}
