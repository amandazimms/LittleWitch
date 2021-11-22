using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class SuppliesCount : MonoBehaviour
{
    public int numPotions = 10;
    public Text potionsNumber;
    public int numPlants = 5;
    public Text plantsNumber;

    public UnityEvent OnPotionCountZero;
    public UnityEvent OnPotionCountPositive;
    public UnityEvent OnPlantCountZero;
    public UnityEvent OnPlantCountPositive;

    public void AddPotion()
    {
        numPotions++;
        potionsNumber.text = numPotions.ToString();

        if (numPotions > 0)
            if (OnPotionCountPositive != null)
                OnPotionCountPositive.Invoke();
    }

    public void SubtractPotion()
    {
        numPotions--;
        potionsNumber.text = numPotions.ToString();

        if (numPotions == 0)
            if (OnPotionCountZero != null)
                OnPotionCountZero.Invoke();
    }

    public void AddPlant()
    {
        numPlants++;
        plantsNumber.text = numPlants.ToString();

        if (numPlants > 0)
            if (OnPlantCountPositive != null)
                OnPlantCountPositive.Invoke();
    }

    public void SubtractPlant()
    {
        numPlants--;
        plantsNumber.text = numPlants.ToString();

        if (numPlants == 0)
            if (OnPlantCountZero != null)
                OnPlantCountZero.Invoke();
    }

}
