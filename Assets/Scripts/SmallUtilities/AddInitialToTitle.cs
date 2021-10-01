using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddInitialToTitle : MonoBehaviour
{
    //Add this script to "title" UI elements.
    //It will remove and store the first letter of the title,
    //create a new text game object where that was,
    //and set its font to our fancy medieval initial font
    //and also reposition so the entire title appears centered.

    public Text originalTitle;
    public GameObject initialPrefab;
    public Canvas canvas;

    GameObject initialTextGO;

    public string firstLetter;
    public string restOfTitle;

    float screen_scale;

    Vector2 worldPos;

    void Awake()
    {
        screen_scale = canvas.scaleFactor;
        AddInitial();
    }

    public void AddInitial()
    {
        string text = originalTitle.text;

        if (0 >= text.Length)
            return;

        TextGenerator textGen = new TextGenerator(text.Length);
        Vector2 extents = originalTitle.gameObject.GetComponent<RectTransform>().rect.size;
        textGen.Populate(text, originalTitle.GetGenerationSettings(extents));

        int newLine = text.Substring(0, 0).Split('\n').Length - 1;
        int whiteSpace = text.Substring(0, 0).Split(' ').Length - 1;
        int indexOfTextQuad = (0 * 4) + (newLine * 4) - (whiteSpace * 4);
        if (indexOfTextQuad < textGen.vertexCount)
        {
            Vector3 avgPos = (textGen.verts[indexOfTextQuad].position +
                textGen.verts[indexOfTextQuad + 1].position +
                textGen.verts[indexOfTextQuad + 2].position +
                textGen.verts[indexOfTextQuad + 3].position) / 4f;

            worldPos = originalTitle.transform.TransformPoint(avgPos / screen_scale);

            GameObject initialTextGO = Instantiate(initialPrefab, new Vector3(worldPos.x, worldPos.y, 0), Quaternion.identity, gameObject.transform);

            RectTransform rect = initialTextGO.GetComponent<RectTransform>();
            rect.localPosition = new Vector3(rect.localPosition.x - 130, rect.localPosition.y + 50, 0);

            firstLetter = originalTitle.text.Substring(0, 1);
            restOfTitle = originalTitle.text.Substring(1, originalTitle.text.Length - 1);
        }
        else
            Debug.LogError("Out of text bound");
    }


    void Update()
    {
        screen_scale = canvas.scaleFactor;
    }
}

