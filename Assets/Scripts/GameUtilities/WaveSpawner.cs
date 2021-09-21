using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    //spawns peasants, but only at night

    public GameObject peasantPrefab;
    public Transform spawnPoint;

    GameObject gameManager;
    DayInfo dayInfo;

    [Tooltip("How long to wait between spawning each peasant (on day 1)?")]
    public int timeBetweenPeasants = 4;


    public List<GameObject> spawnedPeasants;

    public void Start()
    {
        gameManager = GameObject.FindWithTag("GameManager");
        dayInfo = gameManager.GetComponent<DayInfo>();

        StartCoroutine(SpawnOrWaitForNight());

    }

    public IEnumerator SpawnOrWaitForNight()
    {
        if (dayInfo.currentTime == DayInfo.TimeOfDay.Night)
            StartCoroutine(PeasantSpawnCountdown());

        else {
            yield return new WaitForSeconds(1);
            StartCoroutine(SpawnOrWaitForNight());
        }
    }

    public IEnumerator PeasantSpawnCountdown()
    {
        SpawnPeasant();
        int count = timeBetweenPeasants;

        while (count > 0)
        {
            yield return new WaitForSeconds(1);
            count--;
        }
        StartCoroutine(SpawnOrWaitForNight());
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
