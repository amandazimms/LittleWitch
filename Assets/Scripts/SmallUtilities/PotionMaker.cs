using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionMaker : MonoBehaviour
{
    //script that lives on Game Manager (?)
    //when LW clicks "give potion: __{e.g. Saltwater Brew}__",
    //this script instantiates that potion in her hand

    public PotionType thisPotionType;
    public enum PotionType { SaltwaterBrew, BottledBurning, WindblownTonic, EarthyElixir }

    public Color saltwaterBrewColor, bottledBurningColor, windblownTonicColor, earthyElixirColor;

    void Awake()
    {
        
    }

    public void SetPotionType(PotionType potionTypeToSet)
    {
        switch (potionTypeToSet)
        {
            case PotionType.SaltwaterBrew:

                break;
        }
    }

}
