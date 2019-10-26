using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class SelectionManager : MonoBehaviour
{
    /// this is the master script that oversees all the Selectables on individual GOs.
    /// must set selectableLayer to selectable in inspector
    /// lives on GameManager with tag
    /// handles all the clickiness and rays, and keeps track of the ONE current selection and its Selectable script.
    /// also houses the master variables for glowiness, color and speed, that get applied to individual selections (consistently!)

    [HideInInspector]public bool isTouchBuild = false;  //if false, assume PC
    public Selectable currentSelection;
    public Selectable previousSelection;
    public Selectable currentMousedOver;
    public List<Selectable> population; //list of all the Selectable objects in the scene. Selectable script self-polulates this.

    SelectionMenu selectionMenu;

    public LayerMask selectableLayer;

    public Color glowColor;
    public float glowSpeed = .4f;

    Ray ray;
    GameObject clickedGO;
    PlayerStats playerStats;
    PlayerMovement playerMovement;

    public UnityEvent OnASelect; //cautous that I would cause issues naming these OnSelect, OnMouseEnter, etc.
    public UnityEvent OnADeselect; //since those are built in functions already
    public UnityEvent OnADeselectPrevious; //...
    public UnityEvent OnAMouseEnter; //so naming them all with an A
    public UnityEvent OnAMouseExit; //to avoid confusion

    public Stats mostRecentStatsInteractedWith;
    public Stats previousStatsInteractedWith;
    public string mostRecentInteractMethodName;
    public string previousInteractMethodName;
    //public int mostRecentInteractNumber; //todelete?
    //public int previousInteractNumber; //todelete?

    public RaycastHit2D[] hits;
    public List<Collider2D> cols;
    public List<SortingGroup> sGs;

  
    void Awake()
    {
        selectionMenu = GameObject.FindWithTag("SelectionMenu").GetComponent<SelectionMenu>();
        GameObject player = GameObject.FindWithTag("Player");
        playerStats = player.GetComponent<PlayerStats>();
        playerMovement = player.GetComponent<PlayerMovement>();

        playerMovement.OnNewDestinationWorldPoint.AddListener(OnPlayerNewDestWorld);
    }

    public void InteractClicked(Stats newClickStats, string newClickMethodName, int newClickRouteNumber) // gets called automatically by selectionMenu when any button is clicked.
    {
        //shift these values over one, so the mostRecent becomes previous, and newest becomes mostRecent
        previousStatsInteractedWith = mostRecentStatsInteractedWith;
        previousInteractMethodName = mostRecentInteractMethodName;

        mostRecentStatsInteractedWith = newClickStats;
        mostRecentInteractMethodName = newClickMethodName;

        //also assign the 'new click' number to the stats' routeNumber.
        //newClickStats.mostRecentInteractNumber = newClickRouteNumber;

        if (previousStatsInteractedWith != null && previousStatsInteractedWith.playerEnrouteToMe == true)
        {
            previousStatsInteractedWith.StopCoroutine(previousInteractMethodName);
            previousStatsInteractedWith.waitForRoute = false; //remove these next 3 to undo SaturdaySol 
            previousStatsInteractedWith.playerEnrouteToMe = false;
            previousStatsInteractedWith.playerReachedMe = false;
        }
    }

    public void OLDInteractClicked(Stats newClickStats, int newClickRouteNumber) //delete this nasty mess as soon as you're sure we're not using it.
    {
        /*
        print("interact clicked");
        //shift these values over one, so the mostRecent becomes previous, and newest becomes mostRecent
        previousStatsInteractedWith = mostRecentStatsInteractedWith;
        previousInteractNumber = mostRecentInteractNumber;

        mostRecentStatsInteractedWith = newClickStats;
        mostRecentInteractNumber = newClickRouteNumber;

        //also assign the 'new click' number to the stats' routeNumber.
        newClickStats.mostRecentInteractNumber = newClickRouteNumber;

        //if we had a previous interact before this AND that object is waiting for a route AND it's a different interact than the current one
        //(the 3rd condition prevents this from triggering if you interrupt yourself by interacting with the same object twice)
        if (previousStatsInteractedWith != null && previousStatsInteractedWith.waitForRoute && previousInteractNumber != newClickStats.mostRecentInteractNumber)
        {
            print("interrupt. prev stats is " + previousStatsInteractedWith + ", is waiting? " + previousStatsInteractedWith.waitForRoute + ". Prev interact # " + previousInteractNumber + " new # " + newClickStats.mostRecentInteractNumber);
            previousStatsInteractedWith.playerStartedNewInteract = true;
        }
        print("end of interact clicked"); */
    }

    public void OnPlayerNewDestWorld()
    {
        if (mostRecentStatsInteractedWith != null && mostRecentStatsInteractedWith.playerEnrouteToMe == true)
        {
            mostRecentStatsInteractedWith.StopCoroutine(mostRecentInteractMethodName);
            mostRecentStatsInteractedWith.waitForRoute = false; //remove these next 3 to undo SaturdaySol 
            mostRecentStatsInteractedWith.playerEnrouteToMe = false;
            mostRecentStatsInteractedWith.playerReachedMe = false;
        }

        //old:
        //if (mostRecentStatsInteractedWith != null && !mostRecentStatsInteractedWith.playerEnrouteToMe) //if player was enroute somewhere when this interact was clicked
        //  mostRecentStatsInteractedWith.playerStartedNewInteract = true;
    }

    void Start()
    {
    }

    void Update()
    {
        if (!playerStats.isFrozenFromInteractingAndMoving)
        {
            if (isTouchBuild)
                LookForTouches();
            else 
                LookForClicks();
        }
    }

    public void LookForClicks()
    {
        if (Input.GetMouseButtonDown(0))  //if we left clicked
        {
            if (EventSystem.current.IsPointerOverGameObject()) //check to make sure we're not clicking a UI element (so we click that instead of performing selection)
                return; //if it was a UI element, leave selection method (so that UI functions can execute instead)

            RayClicker(Input.mousePosition);
        }
    }

    public void LookForTouches()
    {
        if (Input.touchCount == 1)  //if we touched the screen
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                if (EventSystem.current.IsPointerOverGameObject(touch.fingerId)) //check to make sure we're not clicking a UI element (so we click that instead of performing selection)
                    return;//if it was a UI element, leave selection method (so that UI functions can execute instead)

                RayClicker(touch.position);
            }
        }
    }

    public void RayClicker(Vector3 inputPos)
    {
        sGs.Clear();
        cols.Clear();
 
        /*  toDelete; ye olde simpler way of clicking stuff which had issues with overlapping colliders. 
        hit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition), Mathf.Infinity, selectableLayer); //detect any objects in the ray's path ON THE SELECTION LAYER
        
        if (hit.collider != null)  //if anything was detected
        {
                clickedGO = hit.collider.gameObject; //name that thing clickedGO
                TryToSelectIt(clickedGO);
            }

            else if (hit.collider == null) //if nothing was detected...
            {
                if (currentSelection != null) //...and we had something selected, we must be trying to click away to deselect the current selection
                    DeselectIt(currentSelection);
            }     

        //clickedGO = hit.collider.gameObject; //name that thing clickedGO
        //TryToSelectIt(clickedGO);
        */

        hits = Physics2D.GetRayIntersectionAll(Camera.main.ScreenPointToRay(inputPos), Mathf.Infinity, selectableLayer); //detect any objects in the ray's path ON THE SELECTION LAYER            

        if (hits != null && hits.Length > 0) 
        {
            foreach (RaycastHit2D hit1 in hits)
                if (hit1.collider != null)  //if anything was detected
                    cols.Add(hit1.collider); //add it to the list of hit colliders
        }

        if (cols.Count == 0) //the list is still empty it means nothing was detected...
        {
            if (currentSelection != null) //...if we had something selected, we must be trying to click away to deselect the current selection
                DeselectIt(currentSelection);
        }

        else if (cols != null && cols.Count > 0) //if we hit one or more colliders with our click
        {
            SortingGroup highestSG = null;

            for (int i = 0; i < cols.Count; i++) //loop through the colliders
            {
                SortingGroup sG = cols[i].gameObject.GetComponent<SortingGroup>(); //check is there's a SG on the collider

                if (sG != null) //if there is,
                    sGs.Insert(i, sG); //insert it into the sGs list at the same index as the cols list.

                else
                    sGs.Add(null); //add an empty item to the sGs list so it maintains the same length as cols list.
            }

            if (sGs != null && sGs.Count > 0)
            {
                int maxID = -99999999; //by setting this low, the first iteration of the for loop will always set sGs[0] to max. Then the others will be compared against it. 

                for (int i = 0; i < sGs.Count; i++)
                {
                    if (sGs[i] != null)
                    {
                        string sGiLayer = sGs[i].sortingLayerName; //see dictionary at bottom of class. We scoop out the layer name from the sG.

                        if (SortingLayers[sGiLayer] > maxID) //with dictionary magic, plug that layer name in to get the int value we attached to it in the dictionary. if current index is higher than max
                        {
                            maxID = SortingLayers[sGiLayer]; //set our definition of what max is to sl's dictionary value. 
                            highestSG = sGs[i]; //also set this sortingGroup to the highestSG status.
                        }

                        else if (SortingLayers[sGiLayer] == maxID) //if current index is the same as max, they're on the same sorting layer.
                        {
                            if (sGs[i].sortingOrder > highestSG.sortingOrder) //so now we should compare the sorting ORDER instead. 
                            {
                                maxID = SortingLayers[sGiLayer]; //set our definition of what max is to its value. 
                                highestSG = sGs[i]; //also set this sortingGroup to the highestSG status.
                            }
                            //note that we don't need an if statement for the opposite (<), since those results are already the case. also fine if these results are the case in a very unlikely == scenario.
                        }
                    }
                }
            }

            clickedGO = highestSG.gameObject; //name that thing clickedGO
            TryToSelectIt(clickedGO);
        }
    }

    public void TryToSelectIt(GameObject _clickedGO)
    {
        previousSelection = currentSelection;

        currentSelection = clickedGO.GetComponent<Selectable>(); //get the script "selection" that belongs to clickedGO,

        if (currentSelection == null) //if no selectable script was detected we must have clicked a tree or something which isn't actually selectable, so get out of here.
            return;

        if (currentSelection.isSelected == true) //if it was already selected when you clicked
            DeselectIt(currentSelection); //we must be trying to deselect, so deselect it.

        else if (currentSelection.isSelected == false) //finally, if it wasn't yet selected, we must be wanting to click it!
            SelectIt();
    }

    public void SelectIt()
    {
        currentSelection.SelectMe(); //run SelectMe on it's Selectable script (make it the selected object)

        if (previousSelection != null)
            DeselectPrevious(); //since we can't have two things selected at once

        if (OnASelect != null) //I think important to do this after DeselectPrevious since deselecting makes things disappear
            OnASelect.Invoke(); //and selecting makes them appear, they would never appear if in the other order.
    }

    public void DeselectIt(Selectable selectableToDeselect) //also called from berriesStats etc.
    {
        if (OnADeselect != null)
            OnADeselect.Invoke();

        previousSelection = currentSelection;
        currentSelection = null; //(for this script's info)

        selectableToDeselect.DeselectMe();//(for the selectable's script's info)
    }

    public void DeselectPrevious()
    {
        if (OnADeselectPrevious != null)
            OnADeselectPrevious.Invoke();

        previousSelection.DeselectMe();
    }

    public void MouseEnter(Selectable mousedOver) //gets called from Selectable in a regular OnMouseEnter
    {
        currentMousedOver = mousedOver;

        if (OnAMouseEnter != null)
            OnAMouseEnter.Invoke();
    }

    public void MouseExit() //gets called from Selectable in a regular OnMouseExit
    {
        currentMousedOver = null;

        if (OnAMouseExit != null)
            OnAMouseExit.Invoke();
    }


    private static Dictionary<string, int> SortingLayers = new Dictionary<string, int>(10);

    private void OnEnable()
    {
        SortingLayers.Add("Default", 0);
        SortingLayers.Add("Background", 1);
        SortingLayers.Add("Main", 2);
        SortingLayers.Add("PotionLiquid", 3);
        SortingLayers.Add("PotionTextures", 4);
        SortingLayers.Add("PotionDraining", 5);
        SortingLayers.Add("ForegroundTrees", 6);
        SortingLayers.Add("HutInsideBG", 7);
        SortingLayers.Add("HutInsideMain", 7);
        SortingLayers.Add("UI", 9);
        SortingLayers.Add("Cursor", 10);
    }
}

















