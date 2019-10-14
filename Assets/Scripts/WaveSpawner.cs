using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public GameObject peasantPrefab;
    public Transform spawnPoint;

    public float timeBetweenPeasants = 5f;
    public float countdown = 2f;

    public List<GameObject> spawnedPeasants;

    private void Update()
    {
        if (countdown <= 0)
        {
            SpawnPeasant();
            countdown = timeBetweenPeasants;
        }

        countdown -= Time.deltaTime; //reduces countdown by 1 every second 
    }

    void SpawnPeasant()
    {
        GameObject thisPeasant = Instantiate(peasantPrefab, spawnPoint.position, Quaternion.identity);
        spawnedPeasants.Add(thisPeasant);

        thisPeasant.name = thisPeasant.name + (spawnedPeasants.Count - 1);

        MoveAlongPath thisPeasantsMover = thisPeasant.GetComponent<MoveAlongPath>();
        thisPeasantsMover.waveSpawner = this;

        if (spawnedPeasants.Count >= 2)
            thisPeasantsMover.peasantInFrontOfMe = spawnedPeasants[spawnedPeasants.Count - 2];
    }
}
