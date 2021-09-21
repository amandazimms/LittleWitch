using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class SelectionMenu : MonoBehaviour
{
    /// this is for the "the sims social menu"-like thing that appears on selected objects. analogous to NameDisplay but more complicated.
    /// it controls ONE SelectionMenu GO which it lives on 
    /// and that hops around to the currently selected object, and appears when there's a selection / disappears when not
    /// this asks selectionManager about what's selected
    /// buttons are populated with appropriate contents in the berriesStats, foxStats, playerStats scripts. 

    public Animator anim;
    public SelectionManager selectionManager;
    public Selectable currentSelection;
    public Canvas canvas; //displays the buttons('s game objects)
    public ShapeVariance[] myShapeVariance;

    public Vector3 baseScale; //0.015 for each

    Camera mainCam;

    public GameObject[] actButtGO; //array of gameobjects that house the action buttons. 
    public Button[] actButtButt; //deeper! array of action buttons themselves; that live inside above. 
    public Text[] actButtText; //deeper still! array of text labels on buttons; that live inside the above.

    public ContentSizeFitter[] contentSizeFitters;
    public HorizontalLayoutGroup[] horizontalLayoutGroups;

    public RectTransform[] rectTransforms;

    public UnityEvent OnInteractClicked;

    public float zoomYoffset;
    public float zoomMultiplierOffset;


    void Awake()
    {
        GameObject gameManager = GameObject.FindWithTag("GameManager");
        selectionManager = gameManager.GetComponent<SelectionManager>();
        selectionManager.OnASelect.AddListener(OnSelectionManagerOnSelected);
        selectionManager.OnADeselect.AddListener(OnSelectionManagerOnDeselected);
        mainCam = Camera.main;

        myShapeVariance = GetComponentsInChildren<ShapeVariance>();

        canvas.enabled = false;
    }

    void Start()
    {
    }

    void Update()
    {
        MoveSelectMenuToSelection();
        SizeToZoom();
    }

    void SizeToZoom()
    {
        transform.localScale = new Vector3((mainCam.orthographicSize + zoomMultiplierOffset) * baseScale.x, (mainCam.orthographicSize + zoomMultiplierOffset) * baseScale.y, (mainCam.orthographicSize + zoomMultiplierOffset) * baseScale.z);
    }

    void MoveSelectMenuToSelection()
    {
        currentSelection = selectionManager.currentSelection; //todo call this in an event instead

        if (currentSelection != null)
        {
            Transform currSelTrans = currentSelection.transform;
            transform.position = currSelTrans.position + (currentSelection.menuOffset * (currSelTrans.localScale.y + (mainCam.orthographicSize * zoomYoffset))); 

            //transform.position = new Vector3(currSelTrans.position.x, currSelTrans.position.y + 3f, currSelTrans.position.z); //TODO change 3 to some variable offset so to account for tall GOs. //TODO y of -3 is hard coded into the animations for popUp and popDown
        }
    }

    void OnSelectionManagerOnSelected()
    {
        Appear();
    }

    void OnSelectionManagerOnDeselected()
    {
        Disappear();
    }

    public void Appear()
    {
        anim.SetTrigger("popUp");
    }

    public void Disappear()
    {
        anim.SetTrigger("popDown");
    }

    public void DisableCanvas()
    {
        canvas.enabled = false;
    }

    public void EnableCanvas()
    {
        foreach (ShapeVariance var in myShapeVariance)
        {
            var.RollRandoms();
            var.SetShape();
        }

        canvas.enabled = true;
    }

    public void PopulateButton(int buttonCount, string label, UnityAction action, string methodName, Stats stats)
    {
        actButtText[buttonCount].text = label;
        actButtButt[buttonCount].onClick.RemoveAllListeners();
        actButtButt[buttonCount].onClick.AddListener(delegate { ButtonWasClicked(stats, methodName); });
        actButtButt[buttonCount].onClick.AddListener(action);
        actButtGO[buttonCount].SetActive(true);
        actButtButt[buttonCount].interactable = true;
    }

    public void DeactivateAllButtonGOs() //in berriesStats etc, this method is always called before populating relevant buttons, to make a clean slate
    {
        actButtGO[0].SetActive(false);
        actButtGO[1].SetActive(false);
        actButtGO[2].SetActive(false);
        actButtGO[3].SetActive(false);
        actButtGO[4].SetActive(false);
    }


    public void ButtonWasClicked(Stats _stats, string _methodName) //gets called at the moment any interact button is clicked. 
    {
        selectionManager.InteractClicked(_stats, _methodName, Random.Range(-1000,1000));
        //replace above with: selectionManager.InteractClicked(_stats, _methodName); when ready to switch 
        //if (OnInteractClicked != null)
            //OnInteractClicked.Invoke();
    }
}
