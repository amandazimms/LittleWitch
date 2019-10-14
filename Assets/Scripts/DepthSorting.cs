using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode] 
[RequireComponent(typeof(SortingGroup))]

public class DepthSorting : MonoBehaviour
{
    //this bb makes all the sprites appear in front of / behind each other as the SHOULD!
    // REQUIRES all the parent GOs to have a sorting group, and that they are all on the same layer (I use main)

    private const int yMultiplier = 100;
    //might need a higher number if your sprites are larger

    [Tooltip("Check true if the object doesn't move (tree, boulder)")]
    public bool isStatic = false;

    SortingGroup sortGroup;

    void Awake()
    {
        sortGroup = GetComponent<SortingGroup>();
    }

    private void Start()
    {
        sortGroup.sortingOrder = -(int)(transform.position.y * yMultiplier);
    }

    void Update()
    {
        if (!isStatic)
            sortGroup.sortingOrder = -(int)(transform.position.y * yMultiplier);
    }
}

