using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    [HideInInspector] public bool isTouchBuild = false;

    [HideInInspector] public bool isMoving = false;
    [HideInInspector] public bool closeToDest = false;

    public float speed = 1;
    public float animSpeed = .75f;

    public Animator anim;

    public GameObject moveTargetPrefab;
    public Transform target; //transform of the moveTarget

    [HideInInspector] public Vector3[] path;
    int targetIndex;
    Vector3 currentWaypoint;


    GridSeb gridSeb;
    public float gridNodeRadius; //I think this similar to a collider; how big is the player in terms of fitting through things on the grid for pathfinding?

    public Vector3 myScale; //[HideInInspector]  //set by PlayerStats
    PlayerStats playerStats;
    Rigidbody2D rigidBod;

    [HideInInspector] public UnityEvent OnNewDestinationWorldPoint; //will get called when (first) gets a world point destination (NOT bush, mouse)
    [HideInInspector] public UnityEvent OnReachedDestinationWorldPoint; //will get called upon reaching a world point destination (NOT bush, mouse)

    //todo [hide
    public GameObject objectTarget; //if player is moving toward a specific object (berries), it's stored here. This also keeps track of WHETHER there's an object target (is it null or not)
    [HideInInspector] public UnityEvent OnNewDestinationObject; //will get called when (first) gets a new object destination (bush, mouse)
    [HideInInspector] public UnityEvent OnReachedDestinationObject; //will get called upon reaching object destination (bush, mouse)

    [HideInInspector] public UnityEvent OnPlayerStartedMoving; //will get called when player starts moving toward world point or bush/mouse
    [HideInInspector] public UnityEvent OnPlayerStoppedMoving; //will get called when player reaches world point or bush/mouse
                                                               //not sure if the last 2 are redundant to the first 4; maybe this was part of my solution to the bug when picking a dest too close to current pos 

    Stats[] allStats;

    private void Awake()
    {
        GameObject gameManager = GameObject.FindWithTag("GameManager");
        gridSeb = gameManager.GetComponent<GridSeb>();
        gridNodeRadius = gridSeb.nodeRadius;
        rigidBod = GetComponent<Rigidbody2D>();

        playerStats = GetComponent<PlayerStats>();

        GameObject targetGO = Instantiate(moveTargetPrefab);
        target = targetGO.transform;

    }

    private void Start()
    {
    }

    void Update()
    {
        if (!playerStats.isFrozenFromInteractingAndMoving && !playerStats.isFrozenFromMoving)
        {
            if (isTouchBuild)
                GetTouches();
            else if (!isTouchBuild)
                GetClicks();
        }
        AreWeCloseToDest();
    }

    public void MoveToObject(Vector3 targetPos, GameObject _objectTarget) //for moving specifically toward a target object (e.g. blackberries, mouse)
    {
        objectTarget = _objectTarget;
        target.position = targetPos;

        PathRequestManager.RequestPath(transform.position, targetPos, OnPathFound);

        if (OnNewDestinationObject != null)
            OnNewDestinationObject.Invoke();
    }

    void GetClicks()
    {
        if (Input.GetMouseButtonDown(1)) //if we right clicked
        {
            if (EventSystem.current.IsPointerOverGameObject()) //check to make sure we're not clicking a UI element (so we click that instead of performing selection)
                return; //if it was a UI element, leave selection method (so that UI functions can execute instead)

            //not using this now, but will need it if we switch to all left clicks.
            //if (IsInputOverCollider(Input.mousePosition)) //if we clicked something with a collider, get out of here (because we were trying to select, not move)
            //return;

            Vector3 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPos.z = 0; //needed this line because camera Z is at -10 and mouseclick would register at -10 also.

            target.position = targetPos;

            PathRequestManager.RequestPath(transform.position, targetPos, OnPathFound);

            //if (OnNewDestinationWorldPoint != null)
                //OnNewDestinationWorldPoint.Invoke();
        }
    }

    void GetTouches()
    {
        if (Input.touchCount == 1)  //if we touched the screen
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                if (EventSystem.current.IsPointerOverGameObject(touch.fingerId)) //check to make sure we're not clicking a UI element (so we click that instead of performing selection)
                    return;//if it was a UI element, leave selection method (so that UI functions can execute instead)

                if (IsInputOverCollider(touch.position))
                    return;

                Vector3 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                targetPos.z = 0; //needed this line because camera Z is at -10 and mouseclick would register at -10 also.

                target.position = targetPos;

                PathRequestManager.RequestPath(transform.position, targetPos, OnPathFound);

                if (OnNewDestinationWorldPoint != null)
                    OnNewDestinationWorldPoint.Invoke();
            }
        }
    }

    bool IsInputOverCollider(Vector3 inputPos)
    {
        RaycastHit2D hit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(inputPos), Mathf.Infinity); //detect any objects in the ray's path ON THE SELECTION LAYER
        if (hit.collider)  //if anything was detected
            return true;

        else
            return false;
    }

    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = newPath;
            targetIndex = 0;
            StopCoroutine("FollowPath"); //IMPORTANT - do not make these IEnumerators instead of strings, everything breaks
            StartCoroutine("FollowPath"); //IMPORTANT - do not make these IEnumerators instead of strings, everything breaks
        }

        if (!pathSuccessful)
        {
            print("path unsuccessful for player; retrying new path");
            TrySimilarPath();
        }
    }

    void TrySimilarPath()
    {
        /// so far in my testing no path found only happens if the target is inside a collider (and not on the walkable points grid).
        /// if this happens, we'll scoot the target away a little and try again. Repeat until we find a path.
        /// definitely not bulletproof but simply solves the most common issues.

        Vector3 addVector;
        float rand = Random.Range(0f, 1f);

        if (rand <= .25f)
            addVector = new Vector3(-0.7f, 0, 0);
        if (rand > .25f && rand <= .5f)
            addVector = new Vector3(0.7f, 0, 0);
        if (rand > .5f && rand <= .75f)
            addVector = new Vector3(0, 0.4f, 0);
        else
            addVector = new Vector3(0, -0.4f, 0);

        target.position += addVector;
        PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
    }

    public IEnumerator FollowPath()
    {
        if (path.Length != 0) //if we're walking to a new place, we're standing on a different node than the target node, so there will be a path to get there.
            currentWaypoint = path[0]; //so set first element in path[] to the current waypoint.

        else //if our target is on the same node we're standing on (very close / not really walking to a new place), there's no "path" to get there and path.Length will be 0.
            currentWaypoint = gridSeb.NodeFromWorldPoint(transform.position).worldPosition; //see #note A# - set the current waypoint to our transform since there's no path to set it to.

        if (OnPlayerStartedMoving != null)
            OnPlayerStartedMoving.Invoke();

        while (true)
        {
            //print("playermovement. followPath while");
            FlipSprite();

            isMoving = true;
            anim.SetFloat("moveSpeed", speed * animSpeed);

            if (transform.position == currentWaypoint) //we will jump straight inside this on first path if we did #note A#.
            {
                targetIndex++;

                if (targetIndex >= path.Length) //if #note A#, this will always be true since path.Length = 0 and targetIndex is at least 1
                {
                    if (objectTarget == null) //if we had a world point destination
                    {
                        if (OnReachedDestinationWorldPoint != null)
                            OnReachedDestinationWorldPoint.Invoke();
                    }

                    else if (objectTarget != null) //if we had an object target destination
                    {
                        if (OnReachedDestinationObject != null)
                            OnReachedDestinationObject.Invoke();

                        objectTarget = null;
                    }

                    if (OnPlayerStoppedMoving != null)
                        OnPlayerStoppedMoving.Invoke();

                    isMoving = false;
                    anim.SetFloat("moveSpeed", speed * 0f);

                    targetIndex = 0; //new
                    path = new Vector3[0]; //new

                    yield break;
                }
                currentWaypoint = path[targetIndex];
            }
            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);

            /*
            float velocity = Time.deltaTime;
            float currentSpeed = 0;
            float dampedSpeed = Mathf.SmoothDamp(currentSpeed, speed, ref velocity, 1f);

            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, dampedSpeed * 8f * Time.deltaTime);
            */
            /*
            Vector3 movePosition = transform.position;
            movePosition = Vector3.MoveTowards(transform.position, currentWaypoint, speed * 8f * Time.deltaTime);
            rigidBod.MovePosition(movePosition); */

            yield return null;
        }
    }

    void AreWeCloseToDest()
    {
        if (Vector3.Distance(transform.position, target.position) <= 1f && isMoving)
            closeToDest = true;

        else if (Vector3.Distance(transform.position, target.position) > 1f || Vector3.Distance(transform.position, target.position) < 0.02f || !isMoving)
            closeToDest = false;
    }

    public void FlipSprite()
    {
        if (transform.position.x < currentWaypoint.x)
            transform.localScale = new Vector3(myScale.x * 1, myScale.y, myScale.z);

        if (transform.position.x > currentWaypoint.x)
            transform.localScale = new Vector3(myScale.x * -1, myScale.y, myScale.z);
    }

    public void OnDrawGizmos()
    {
        if (path != null)
        {
            for (int i = targetIndex; i < path.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], Vector3.one);

                if (i == targetIndex)
                    Gizmos.DrawLine(transform.position, path[i]);
                else
                    Gizmos.DrawLine(path[i - 1], path[i]);
            }
        }
    }








    /////////////Little Witch Original below


    /*
	public bool isTouchBuild;
	public float speed = 5;
    public float animSpeed = .75f;
	//public float jumpStrength = 10;
	public Animator anim;
	public Rigidbody2D myRigidBod;

	public Vector3 targetPos;

	//public bool readyToJump = true;

	public bool movementIsLocked = false;

	void Awake()
	{
		targetPos = transform.position;
		SwipeDetector.OnSwipe += SwipeDetector_OnSwipe;
	}

	void Update()
	{
		if (!movementIsLocked)
			Movement();
	}

	private void SwipeDetector_OnSwipe(SwipeData data)
	{
		if (data.Direction == SwipeDirection.Up)
        { 
			//Vector2 jumpVelocityToAdd = new Vector2(0f, jumpStrength);
			//myRigidBod.velocity += jumpVelocityToAdd;
		}
	}

	void Movement()
	{

		
        #region keyboard controls
        if (!isTouchBuild)
        {
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                transform.position += Vector3.right * speed * Time.deltaTime;
                FlipRight();
            }
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                transform.position += Vector3.left * speed * Time.deltaTime;
                FlipLeft();
            }
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            {
                transform.position += Vector3.up * speed * Time.deltaTime;
                //Vector2 jumpVelocityToAdd = new Vector2(0f, jumpStrength); //if using this, do getkeyDOWN
                //myRigidBod.velocity += jumpVelocityToAdd; //if using this, do getkeyDOWN
            }
            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            {
                transform.position += Vector3.down * speed * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
                anim.SetFloat("moveSpeed", speed * animSpeed); 

            else
                anim.SetFloat("moveSpeed", 0);
        }
        #endregion keyboard controls



        #region touch and click controls
        if (isTouchBuild)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                targetPos = Camera.main.ScreenToWorldPoint(touch.position);
                targetPos.z = 0;
                //targetPos.y = transform.position.y;

                if (transform.position.x > targetPos.x)
                    FlipLeft();
                else if (transform.position.x <= targetPos.x)
                    FlipRight();
            }

            if (Input.GetMouseButtonDown(0))
            {
                targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                targetPos.z = 0;
                //targetPos.y = transform.position.y;

                if (transform.position.x > targetPos.x)
                    FlipLeft();
                else if (transform.position.x <= targetPos.x)
                    FlipRight();
            }

            if (Vector3.Distance(transform.position, targetPos) >= 0.1f) //instead of just making =, this should account for tiny discrepencies with physics?
            {
                anim.SetBool("isMoving", true);
                transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            }
            else if (Vector3.Distance(transform.position, targetPos) < 0.1f) //if were "at" the destination 
                anim.SetBool("isMoving", false);
        }

        #endregion touch controls
    }

	public void FlipRight()
	{
		transform.localScale = new Vector3(1, 1, 1);
	}

	public void FlipLeft()
	{
		transform.localScale = new Vector3(-1, 1, 1);
	}
    */
}

