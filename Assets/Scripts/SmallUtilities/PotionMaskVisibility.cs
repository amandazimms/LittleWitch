using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PotionMaskVisibility : MonoBehaviour
{
    public SpriteRenderer[] sprites;
    [Tooltip("0 for no interaction, 1 for inside, 2 for outside")]
    public int[] spriteVisibilityKeyNormal;
    [Tooltip("0 for no interaction, 1 for inside, 2 for outside")]
    public int[] spriteVisibilityKeyCauldron;

    public SortingGroup sg;
    string originalSortingLayer;
    int originalSortingID;

    private void Awake()
    {
        originalSortingLayer = sg.sortingLayerName;
        originalSortingID = sg.sortingLayerID;
    }

    public void SetToCauldronVisibility()
    {
        if (sg)
            Destroy(sg);

        for (int i = 0; i < sprites.Length; i++)
        {
            if (spriteVisibilityKeyCauldron[i] == 0)
                sprites[i].maskInteraction = SpriteMaskInteraction.None;
            if (spriteVisibilityKeyCauldron[i] == 1)
                sprites[i].maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            if (spriteVisibilityKeyCauldron[i] == 2)
                sprites[i].maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
        }
    }

    public void SetToNormalVisibility()
    {
        if (sg)
        {
            Destroy(sg);
        }

       // sg = gameObject.AddComponent<SortingGroup>();
        //sg.sortingLayerID = originalSortingID;
        //sg.sortingLayerName = originalSortingLayer;


        for (int i = 0; i < sprites.Length; i++)
        {
            if (spriteVisibilityKeyNormal[i] == 0)
                sprites[i].maskInteraction = SpriteMaskInteraction.None;
            if (spriteVisibilityKeyNormal[i] == 1)
                sprites[i].maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            if (spriteVisibilityKeyNormal[i] == 2)
                sprites[i].maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
        }
    }
}
