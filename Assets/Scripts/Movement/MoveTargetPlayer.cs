using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTargetPlayer : MonoBehaviour
{
    /// this lives on a GO of the same name, the arrow that appears when you move, to show where you're moving to.
    /// it works in conjunction with PlayerMovement a lot and tries to take care of its own business here so it doesn't clutter that up.
    /// it fades out when you're pretty close; see FadeOut function.

    public SpriteRenderer spRend;
    public Color spRendCol;
    public Animator anim;

    PlayerMovement playerMovement;

    public Camera mainCam;

    Color startColor;
    Color fadeColor;

    float lerpTime = 0;
    Vector3 originalscale;

    void Awake()
    {
        playerMovement = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();

        playerMovement.OnNewDestinationWorldPoint.AddListener(OnPlayerMovementNewDestinationEither);
        playerMovement.OnReachedDestinationWorldPoint.AddListener(OnPlayerMovementReachedDestinationEither);

        //playerMovement.OnNewDestinationObject.AddListener(OnPlayerMovementNewDestinationEither);
        //playerMovement.OnReachedDestinationObject.AddListener(OnPlayerMovementReachedDestinationEither);

        mainCam = Camera.main;

        originalscale = transform.localScale;
    }

    void Start()
    {

    }

    void Update()
    {
        SizeToZoom();
    }

    void OnPlayerMovementNewDestinationEither()
    {
        //print("arrow appear");
        ArrowAppear();
    }

    void OnPlayerMovementReachedDestinationEither()
    {
        ArrowDisappear();
    }

    void LateUpdate()
    {
      if (playerMovement.isMoving)
          CheckDestination(); //has to be in lateUpdate so that colorTimesUI gets a chance to tint it before we mess with alpha
    }

    void SizeToZoom()
    {
        transform.localScale = new Vector3(mainCam.orthographicSize / 7 * originalscale.x, mainCam.orthographicSize / 7 * originalscale.y, mainCam.orthographicSize / 7 * originalscale.z);
    }

    public void ArrowAppear() 
    {
        anim.SetBool("isActive", true);
        anim.SetTrigger("Active");
    }

    public void ArrowDisappear()
    {
        anim.SetBool("isActive", false);
    }

    void CheckDestination()
    {
      if (playerMovement.closeToDest)
          FadeOut();

      else if (!playerMovement.closeToDest)
          ResetAlpha();
    }

    void FadeOut()
    {
        spRendCol = spRend.color; //get the color from our sprite rend and store it in spRendCol.

        float alphaMath = Mathf.Lerp(1, 0, lerpTime);
        lerpTime += Time.deltaTime * 2f; //2 = magic number for the perfect fade

        spRendCol.a = alphaMath;
        spRend.color = spRendCol;
  }

    void ResetAlpha()
    {
        spRendCol.a = 1;
        lerpTime = 0;
    }
}
