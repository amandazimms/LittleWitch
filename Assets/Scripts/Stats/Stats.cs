using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


public class Stats : MonoBehaviour
{
    public Vector3 selectionMenuOffset;

    [HideInInspector] public Selectable selectable;
    [HideInInspector] public SelectionManager selectionManager;
    [HideInInspector] public SelectionMenu selectionMenu;

    [HideInInspector] public DayInfo dayInfo;

    [HideInInspector] public ReputationAvatar reputationMeter;
    [HideInInspector] public SuppliesCount suppliesCount;

    public bool waitForRoute;
    public bool playerEnrouteToMe; //todo [hide
    public int mostRecentInteractNumber; // todelete?
    public bool playerReachedMe; //todo [hide

    [HideInInspector] public PlayerStats playerStats;
    [HideInInspector] public PlayerMovement playerMovement;

    [HideInInspector] public MoveAlongPath moveAlong;
    [HideInInspector] public Animator anim;

    public Vector3 routeToOffset;

    [HideInInspector] public bool hasStartedAnimFinished;
    [HideInInspector] public bool hasStartedAnimReachedKeyMoment;

    public bool isFrozenFromInteractingAndMoving { get; private set; }
    public bool isFrozenFromMoving { get; private set; }
    public bool playerStartedNewInteract = false; //todo [hide or todelete?

    public Vector3 myScale; //[HideInInspector]  //set by TransformVariance, FoxStats, or PlayerStats, whichever exists. 
    public string displayedName = "Default"; //Name. This populates Selectable's Name field and is used by NameDisplay.

    public void StatsAwakeStuff()
    {
        GameObject gameManager = GameObject.FindWithTag("GameManager");
        GameObject selectionMenuGO = GameObject.FindWithTag("SelectionMenu");
        selectionMenu = selectionMenuGO.GetComponent<SelectionMenu>();
        selectionManager = gameManager.GetComponent<SelectionManager>();
        dayInfo = gameManager.GetComponent<DayInfo>();

        reputationMeter = GameObject.FindWithTag("ReputationMeter").GetComponent<ReputationAvatar>();
        suppliesCount = GameObject.FindWithTag("SuppliesCount").GetComponent<SuppliesCount>();


        selectable = GetComponent<Selectable>();
        selectable.menuOffset = selectionMenuOffset;
        selectable.displayedName = displayedName;

        anim = GetComponent<Animator>();

        GameObject player = GameObject.FindWithTag("Player");
        playerStats = player.GetComponent<PlayerStats>();
        playerMovement = player.GetComponent<PlayerMovement>();

        playerMovement.OnNewDestinationWorldPoint.AddListener(OnPlayerMovementNewDestinationWorldPoint);
        playerMovement.OnReachedDestinationWorldPoint.AddListener(OnPlayerMovementReachedDestinationWorldPoint);

        playerMovement.OnNewDestinationObject.AddListener(OnPlayerMovementNewDestinationObject);
        playerMovement.OnReachedDestinationObject.AddListener(OnPlayerMovementReachedDestinationObject);

        myScale = transform.localScale;

        moveAlong = GetComponent<MoveAlongPath>();
        if (moveAlong)
            moveAlong.myScale = myScale;
        PlayerMovement myOwnPlayerMovement = GetComponent<PlayerMovement>();
        if (myOwnPlayerMovement)
            myOwnPlayerMovement.myScale = myScale;
    }


    public void OnPlayerMovementNewDestinationWorldPoint() //if we clicked ground, player is now walking somewhere non-berry, so 
    {
        //Unmute to undo SaturdaySol playerEnrouteToMe = false; //destination can't be me.
    }

    public void OnPlayerMovementReachedDestinationWorldPoint()
    {
        //Unmute to undo SaturdaySol playerEnrouteToMe = false; //is this needed?
    }

    public void OnPlayerMovementNewDestinationObject() //if we are walking toward an object
    {
        if (playerMovement.objectTarget == gameObject) //check if the object is me
            playerEnrouteToMe = true; //if it is, I must be the player's destination

        else if (playerMovement.objectTarget != gameObject) //if not
        {
            //Unmute to undo SaturdaySol playerEnrouteToMe = false; //I must not be.
        }
    }

    public void OnPlayerMovementReachedDestinationObject() //if player reached destination object
    {
        if (playerEnrouteToMe) //and that object is me
            playerReachedMe = true; //player reached me
        //it seems like we should also do playerEnrouteToMe = false here, but careful that might fuck up the entire Interrupt situation over at SelectionManager.InteractClicked();
    }

    public void ChangeReputation(float amount)
    {
        reputationMeter.ChangeReputation(amount);
    }

    public IEnumerator WaitWhilePlayerEnrouteToMe(bool isMovingTarget, bool bothFaceEachother)
    {
        if (playerReachedMe) //its really redundant to have this here but I'm losing my damn mind 
        {
            if (bothFaceEachother)
                FlipAToFaceB(transform, this, playerStats.transform);

            FlipAToFaceB(playerStats.transform, playerStats, transform);

            waitForRoute = false;
            playerEnrouteToMe = false; //so I'm no longer player's destination
            playerReachedMe = false; //so this chunk of code only runs once. //this is redundant with OnPlayer...ReachedDestObj, but see the note after that about not fucking up things. 
        }

        while (playerEnrouteToMe)
        {
            if (isMovingTarget) //&& !playerReachedMe)
            {
                Vector3 playerTarget = CalculatePlayerTarget(transform, routeToOffset);
                playerMovement.MoveToObject(playerTarget, gameObject); //(transform.position, gameObject);
                yield return new WaitForSeconds(Random.Range(.08f, .2f));  //don't do the following unless the player has reached the target
            }
            if (playerReachedMe) //if we reached the target
            {
                if (bothFaceEachother)
                    FlipAToFaceB(transform, this, playerStats.transform);

                FlipAToFaceB(playerStats.transform, playerStats, transform);

                playerEnrouteToMe = false; //so I'm no longer player's destination
                playerReachedMe = false; //so this chunk of code only runs once. //this is redundant with OnPlayer...ReachedDestObj, but see the note after that about not fucking up things. 
                waitForRoute = false;
            }
            yield return null;
        }
    }

    public IEnumerator Anim(bool hasAnim, string animName, bool playerHasAnim, bool playerFirst, string playerAnimName, float secondsToWaitLo, float secondsToWaitHi)
    {
        if (playerHasAnim && playerFirst)
        {
            playerStats.anim.SetTrigger(playerAnimName);
            yield return new WaitForSeconds(Random.Range(secondsToWaitLo, secondsToWaitHi));
        }

        if (hasAnim)
            anim.SetTrigger(animName);

        if (playerHasAnim && !playerFirst)
        {
            yield return new WaitForSeconds(Random.Range(secondsToWaitLo, secondsToWaitHi));
            playerStats.anim.SetTrigger(playerAnimName);
        }
    }

    public void CurrentAnimationFinished()
    {
        hasStartedAnimFinished = true;
    }
    public void CurrentAnimationKeyMoment()
    {
        hasStartedAnimReachedKeyMoment = true;
    }

    public IEnumerator FreezeFromMoving(bool includeInteract, int _secondsToWait, string originatingMethod)
    {
        /// HACKY
        /// normally, any interaction triggers the player to freeze, then after anims have run,
        /// unfreeze with the UnfreezeNaturally method (here). If something goes wrong with pathing,
        /// player can get stuck frozen forever. So this failsafe unfreezes the player automatically after a
        /// certain # of seconds. If all goes well with no sticking, the end of this method isn't needed.
        selectionMenu.Disappear();

        if (includeInteract)
            isFrozenFromInteractingAndMoving = true;

        else if (!includeInteract)
            isFrozenFromMoving = true;

        /* todo - imported from fox fable - replace this with peasant moving stuff for LW?
        if (_critterWander)
            _critterWander.StopForInteractions();
        */

        yield return new WaitForSeconds(_secondsToWait);

        /// at this point, the originating method should already have called UnfreezeFromMoving...
        /// If something went wrong though,player would still be frozen. So check if that's true,
        /// and if so, force an unfreeze.

        
        if (includeInteract)
        {
            if (isFrozenFromInteractingAndMoving && !waitForRoute) //
            {
                isFrozenFromInteractingAndMoving = false;
                // todo replace this with peasant moving stuff for LW?
                     //  if (_critterWander)
                           // _critterWander.ResumeNormalAfterInteraction();
                            //
                playerEnrouteToMe = false;
                print("!!!PROBLEM! " + gameObject.name + "'s FreezeFromMoving unfroze it, saved a stuck-glitch from " + originatingMethod + " from persisting (interact and move).");
            }
        }
        else if (!includeInteract && !waitForRoute)
        {
            if (isFrozenFromMoving)
            {
                isFrozenFromMoving = false;
                // todo replace this with peasant moving stuff for LW?
               // if (_critterWander)
                 //   _critterWander.ResumeNormalAfterInteraction();
                
                playerEnrouteToMe = false;
                print("!!!PROBLEM! " + gameObject.name + "'s FreezeFromMoving unfroze it, saved a stuck-glitch from " + originatingMethod + " from persisting (move).");
            }
        } 
    }

    public void UnfreezeFromMoving(bool includeInteract, string originatingMethod)
    {
        //we call this at the end of each interact. It should be reached. If not, see above (FreezeFromMoving)
        if (includeInteract)
            isFrozenFromInteractingAndMoving = false;
        else if (!includeInteract)
            isFrozenFromMoving = false;
        /* todo replace this with peasant moving stuff for LW?
        if (_critterWander)
            _critterWander.ResumeNormalAfterInteraction();
        */
    }

    public void ForceFreezeMovement(bool includeInteract)
    {
        /// this exists because of dumb planning with InOutBurrowLocks - normally we use FreezeFromMoving to safely
        /// unfreeze after # of seconds, since some anims/paths glitch and get us stuck. This won't work with burrow though
        /// since we don't know how long player will decide to sit in there before hopping out. >:(
        if (includeInteract)
            isFrozenFromInteractingAndMoving = true;
        else if (!includeInteract)
            isFrozenFromMoving = true;

        /* todo replace this with peasant moving stuff for LW?
        if (_critterWander)
            _critterWander.StopForInteractions(); */
    }

    public void FlipAToFaceB(Transform transA, Stats statsA, Transform transB)
    {
        if (transA.position.x <= transB.position.x) //A is to the left of B
            transA.localScale = new Vector3(1 * statsA.myScale.x, statsA.myScale.y, statsA.myScale.z); //flip A to face right (face B)

        else //A is to the right of B
            transA.localScale = new Vector3(-1 * statsA.myScale.x, statsA.myScale.y, statsA.myScale.z); //flip A to face left (face B)
    }

    public Vector3 CalculatePlayerTarget(Transform targetTransform, Vector3 posOffset)
    {
        Vector3 pos = targetTransform.position;
        Vector3 scale = targetTransform.localScale;

        float flipper = 1;

        if (playerStats.transform.position.x > pos.x) //if player is on my right
            flipper = 1;


        else //if player is on my left
            flipper = -1;

        return new Vector3(pos.x + (posOffset.x * Mathf.Abs(scale.x) * flipper), pos.y + (posOffset.y * scale.y), pos.z); //        playerTarget = new Vector3(pos.x + (posOffset.x * flipper * scale.x), pos.y + (posOffset.y * scale.y), pos.z);
    }

    public Vector3 CalculatePlayerTargetExact(Transform targetTransform)
    {
        Vector3 pos = targetTransform.position;
        return new Vector3(pos.x, pos.y, pos.z); 
    }

    public Vector3 CalculatePlayerTargetChild(Transform foodGOTransform, Vector3 posOffset, Transform parentTransformForBerryType) //used for berries, where sub GOs within the parent GO will be collected. 
    {
        Vector3 pos = foodGOTransform.position;
        Vector3 scale = foodGOTransform.localScale;

        if (foodGOTransform.position.x >= parentTransformForBerryType.position.x) // if berry is on right side of bush
            return new Vector3(pos.x + (posOffset.x * scale.x), pos.y + (posOffset.y * scale.y), pos.z);

        else  // if berry is on left side of bush
            return new Vector3(pos.x - (posOffset.x * Mathf.Abs(scale.x)), pos.y + (posOffset.y * scale.y), pos.z);
    }

    void OnDestroy()
    {
        playerMovement.OnNewDestinationWorldPoint.RemoveListener(OnPlayerMovementNewDestinationWorldPoint);
        playerMovement.OnReachedDestinationWorldPoint.RemoveListener(OnPlayerMovementReachedDestinationWorldPoint);

        playerMovement.OnNewDestinationObject.RemoveListener(OnPlayerMovementNewDestinationObject);
        playerMovement.OnReachedDestinationObject.RemoveListener(OnPlayerMovementReachedDestinationObject);
    }

}



