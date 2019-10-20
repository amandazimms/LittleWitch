using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Events;

public class PlayerStats : Stats
{
    public int numPotions;
    public Transform potionCarrySpot; //drag in inspector
    public GameObject potionPrefab;
    public GameObject currentlyCarriedPotion;
    //public CarriedFoodStats currentCarriedFoodStats;

    void Awake()
    {
        StatsAwakeStuff();
        selectable.OnASelected.AddListener(OnSelectableSelected);

        transform.localScale = myScale;
        playerMovement.myScale = myScale;
    }

    void Start()
    {
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            print("========CLEAR===============================");
        }
    }

    public void OnSelectableSelected()
    {
        UpdateSelectionMenu();
    }

    public void UpdateSelectionMenu()
    {
        selectionMenu.DeactivateAllButtonGOs();

        //if we add any interacts here, do like berriesStats et al where  if (playerStats.isInBurrow) { selectionMenu.actButtButt[0].interactable = false; }
    }

    public void PotionAppearInHand()
    {
        if (currentlyCarriedPotion)
            Destroy(currentlyCarriedPotion);

        currentlyCarriedPotion = Instantiate(potionPrefab, potionCarrySpot.position, Quaternion.Euler(0f, 0f, 20f), potionCarrySpot); //potionPrefab, potionCarrySpot.position, Quaternion.identity);
    }






    void OnDestroy()
    {
        selectable.OnASelected.RemoveListener(OnSelectableSelected);
    }


    //this magical bb copies and pastes a component at runtime. TODO put it in its own class?
    Component CopyComponent(Component original, GameObject destination)
    {
        System.Type type = original.GetType();
        Component copy = destination.AddComponent(type);

        // Copied fields can be restricted with BindingFlags
        System.Reflection.FieldInfo[] fields = type.GetFields();

        foreach (System.Reflection.FieldInfo field in fields)
            field.SetValue(copy, field.GetValue(original));

        return copy;
    }
}
