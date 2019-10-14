using System;
using UnityEngine;

public class CameraControls : MonoBehaviour
{
	public bool isTouchBuild = false; //if false, assume PC

	public Camera mainCamera;

	public Vector2 upperRightBounds;
	public Vector2 lowerLeftBounds;
	//public bool setOffsetMultiplierInBuildSettings = true;
	//[Tooltip("If above is true, this will be overwritten by build settings")]

    //todo the zoomClampOffset stuff is only working in a way I can understand for 16:9.
    //Should redefine all limits for a 4:3 resolution instead when ready and just make 16:9 have extra side space.
	public Vector2 zoomClampOffsetMultiplier; //1.76, 1.01 for 16:9
    public Vector2 zoomClampOffset;

	[Header("FOLLOW PLAYER")]
	public Transform target;
	public float damping = .5f;
	public float lookAheadFactor = 1;
	public float lookAheadReturnSpeed = 0.5f;
	public float lookAheadMoveThreshold = 0.1f;

	private float m_OffsetZ;
	private Vector3 lastTargetPos;
	private Vector3 currentVelocity;
	private Vector3 lookAheadPos;

	[Header("ZOOM")]
	public float touchZoomSensitivity = 0.02f; //for touch
	public float touchZoomSpeed = 5f;
	public float zoomSensitivity = 10f;
	public float zoomDamp = .5f;
	public float zoomSpeed = 10f;
	public float zoomMin = 4f;
	public float zoomMax = 9f;
	private float zoom;

	float reff = 10f;

	private void Awake()
	{
		mainCamera = GetComponent<Camera>();
	}

	void Start()
	{
		//FOLLOW PLAYER
		lastTargetPos = target.position;
		m_OffsetZ = (transform.position - target.position).z;
		transform.parent = null;

		//ZOOM
		zoom = mainCamera.orthographicSize;
	}

	void Update()
	{
		/*
        if (Input.GetKeyDown(KeyCode.P)) //for instagram
            target = playerTargetOption;

        if (Input.GetKeyDown(KeyCode.Alpha0)) //for instagram
            target = targetOption2;

        if (Input.GetKeyDown(KeyCode.Alpha1)) //for instagram
            target = targetOption3;
           */

		//FOLLOW PLAYER
		// only update lookahead pos if accelerating or changed direction
		float xMoveDelta = (target.position - lastTargetPos).x;

		bool updateLookAheadTarget = Mathf.Abs(xMoveDelta) > lookAheadMoveThreshold;

		if (updateLookAheadTarget)
			lookAheadPos = lookAheadFactor * Vector3.right * Mathf.Sign(xMoveDelta);

		else
			lookAheadPos = Vector3.MoveTowards(lookAheadPos, Vector3.zero, Time.deltaTime * lookAheadReturnSpeed);

		Vector3 aheadTargetPos = target.position + lookAheadPos + Vector3.forward * m_OffsetZ;
		Vector3 newPos = Vector3.SmoothDamp(transform.position, aheadTargetPos, ref currentVelocity, damping);

		lastTargetPos = target.position;

		//zoomClampOffset = new Vector2(zoomClampOffsetMultiplier.x * upperRightBounds.x / 50 * mainCamera.orthographicSize, zoomClampOffsetMultiplier.y * upperRightBounds.y / 30 * mainCamera.orthographicSize);

        zoomClampOffset = new Vector2((mainCamera.orthographicSize - 2) * zoomClampOffsetMultiplier.x, (mainCamera.orthographicSize - 2) * zoomClampOffsetMultiplier.y);

        newPos.x = Mathf.Clamp(newPos.x, lowerLeftBounds.x + zoomClampOffset.x, upperRightBounds.x - zoomClampOffset.x);
		newPos.y = Mathf.Clamp(newPos.y, lowerLeftBounds.y + zoomClampOffset.y, upperRightBounds.y - zoomClampOffset.y);

		transform.position = newPos;


		if (isTouchBuild)
		{
			if (Input.touchCount == 2)
			{
				// Store both touches.
				Touch touchZero = Input.GetTouch(0);
				Touch touchOne = Input.GetTouch(1);

				// Find the position in the previous frame of each touch.
				Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
				Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

				// Find the magnitude of the vector (the distance) between the touches in each frame.
				float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
				float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

				// Find the difference in the distances between each frame.
				float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

				// ... change the orthographic size based on the change in distance between the touches.
				//mainCamera.orthographicSize += deltaMagnitudeDiff * orthoZoomSpeed;
				zoom += deltaMagnitudeDiff * touchZoomSensitivity;

				// Make sure the orthographic size never exceeds Min/Max
				//mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize, zoomMin, zoomMax); //Mathf.Max(mainCamera.orthographicSize, 0.1f);
				zoom = Mathf.Clamp(zoom, zoomMin, zoomMax);
			}
		}

		else if (!isTouchBuild)
		{
			//ZOOM
			if ((zoom <= zoomMin + .1f && Input.GetAxis("Mouse ScrollWheel") > 0) || (zoom >= zoomMax + .1f && Input.GetAxis("Mouse ScrollWheel") < 0))
			{
				//do nothing
			}

			else
			{
				zoom = Mathf.SmoothDamp(zoom, (zoom - Input.GetAxis("Mouse ScrollWheel") * zoomSensitivity), ref reff, zoomDamp * Time.deltaTime);
				zoom = Mathf.Clamp(zoom, zoomMin, zoomMax);
			}
		}
	}

	void LateUpdate()
	{
		//ZOOM
		if (isTouchBuild)
			mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, zoom, Time.deltaTime * touchZoomSpeed);

		if (!isTouchBuild)
			mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, zoom, Time.deltaTime * zoomSpeed);
	}
}
