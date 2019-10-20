using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    public Camera cam;

    public Transform layer1; //closest to camera
    public float layer1Multi;
    public Transform layer2;
    public float layer2Multi;
    public Transform layer3; //furthest from camera
    public float layer3Multi;

    Vector3 layer1OP, layer2OP, layer3OP;

    private void Awake()
    {
        cam = Camera.main;
        layer1OP = layer1.transform.position;
        layer2OP = layer2.transform.position;
        layer3OP = layer3.transform.position;
    }
    void Update()
    {
        float x3 = cam.transform.position.x * layer3Multi;
        layer3.position = new Vector3(x3, layer3.position.y, layer3.position.z);

        float x2 = cam.transform.position.x * layer2Multi;
        layer2.position = new Vector3(x2, layer2.position.y, layer2.position.z);

        float x1 = cam.transform.position.x * layer1Multi;
        layer1.position = new Vector3(x1, layer1.position.y, layer1.position.z);
    }
}
