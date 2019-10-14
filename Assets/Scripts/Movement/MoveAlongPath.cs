using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAlongPath : MonoBehaviour
{
    public float speed = 10f;
    public float animSpeed = .75f;

    Animator anim;
    [HideInInspector] public WaveSpawner waveSpawner;
    public GameObject peasantInFrontOfMe;

    Transform target;
    int waypointIndex = 0;

    private void Start()
    {
        target = Path.points[0];
        anim = GetComponent<Animator>();


        StartCoroutine("Move"); //must use string
    }

    IEnumerator Move()
    {

        while (true)
        {
            Vector3 dir = target.position - transform.position; 

            if (peasantInFrontOfMe) 
            {
                if (Vector3.Distance(peasantInFrontOfMe.transform.position, transform.position) <= .5f) //this happens when pIFOM has stopped at the end of the path and I catch up to them
                {
                    anim.SetFloat("moveSpeed", 0);
                    yield return null;
                }

                else
                {
                    anim.SetFloat("moveSpeed", speed * animSpeed);
                    transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World);
                }
            }

            else
            {
                anim.SetFloat("moveSpeed", speed * animSpeed);
                transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World);
            }

            if (Vector3.Distance(transform.position, target.position) <= .1f)
                GetNextWaypoint();

            yield return null;
        }
    }

    void GetNextWaypoint()
    {
        if (waypointIndex >= Path.points.Length - 1)
        {
            anim.SetFloat("moveSpeed", 0);
            StopCoroutine("Move"); //must use string
            return;
        }

        else
        {
            waypointIndex++;
            target = Path.points[waypointIndex];
        }
    }
}
