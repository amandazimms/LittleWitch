using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    [HideInInspector] public bool isTouchBuild = false;

    public Camera mainCam;
    public Animator anim;
    public Transform partiTrans;
    public Transform partiTransParent;
    public ParticleSystem parti;

    public SelectionManager selectionManager;
    public SpriteRenderer filledIn;

    public GameObject graphic;

    Color filledInColor;

    void Awake()
    {
        mainCam = Camera.main;
        Cursor.visible = false;
        GameObject gameManager = GameObject.FindWithTag("GameManager");
        selectionManager = gameManager.GetComponent<SelectionManager>();

        if (isTouchBuild)
            graphic.SetActive(false);
        else if (!isTouchBuild)
            graphic.SetActive(true);

        if (partiTransParent)
            partiTrans.SetParent(partiTransParent);
        else
            partiTrans.SetParent(null);
        partiTrans.localScale = new Vector3(.2f, .2f, .2f);
    }

    void Update()
    {
        FollowMouse();
        SizeToZoom();
        AnimateClicks();

        HideCursor(); //todo delete this before build; only a problem with editor and pausing.
    }

    void LateUpdate()
    {
        FilledIn(); //has to be in lateUpdate so that colorTimesUI gets a chance to tint it before we mess with alpha
    }

    void HideCursor()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Cursor.visible = false;
            print("cursor hide");
        }
    }

    void FollowMouse()
    {
        Vector2 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        transform.position = cursorPos;
    }

    void SizeToZoom()
    {
        transform.localScale = new Vector3(mainCam.orthographicSize / 7, mainCam.orthographicSize / 7, mainCam.orthographicSize / 7);
    }

    void AnimateClicks()
    {
        if (Input.GetMouseButtonDown(0)) //if we left clicked
        {
            anim.SetTrigger("LeftClick");
            Vector3 posToMovePartiTransTo = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            posToMovePartiTransTo.z = 0;
            partiTrans.position = posToMovePartiTransTo;
            parti.Play();
        }

        if (Input.GetMouseButtonDown(1)) //if we right clicked
        {
            anim.SetTrigger("RightClick");
            Vector3 posToMovePartiTransTo = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            posToMovePartiTransTo.z = 0;
            partiTrans.position = posToMovePartiTransTo;
            parti.Play();
        }
    }

    void FilledIn()
    {
        filledInColor = filledIn.color; //get the color from our sprite rend and store it in filledInColor.

        if (selectionManager.currentMousedOver != null) //if there is something currentMousedOver
            filledInColor.a = 0; //1

        else if (selectionManager.currentMousedOver == null) //if there's nothign moused over
            filledInColor.a = 0;

        filledIn.color = filledInColor; //constantly update our spRend's color to our Color filledInColor. we do this for the alpha which changes (below)
    }
}
