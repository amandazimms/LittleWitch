using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectVariance : MonoBehaviour
{
    /// This script is IMO sloppier than Transform or Shape Variance.
    /// You build a prefab with different options childed to it (e.g. hair 1, hair 2)
    /// This script will then activate only one at random (and deactivate the rest).

    public GameObject[] options; //drag in inspector

    void Awake()
    {
        ChooseOption();
    }

    void ChooseOption()
    {
        int rand = Random.Range(0, options.Length);

        foreach (GameObject option in options)
            option.SetActive(false);

        options[rand].SetActive(true);
    }
}
