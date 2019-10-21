using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TransformVariance))] //because otherwise no one sets myScale
public class MoveAlongPath : MonoBehaviour
{
    public float speed = 10f;
    public float animSpeed = .75f;

    Animator anim;
    [HideInInspector] public WaveSpawner waveSpawner;
    public GameObject peasantInFrontOfMe;

    Transform target;
    int waypointIndex = 0;

    public bool moveTowardHut; //if false, moveTowardVillage

    public Vector3 myScale; //set by TransformVariance

    private void Start()
    {
        anim = GetComponent<Animator>();
        StartMoveCoroutineToHut();
    }


    public void StartMoveCoroutineToVillage()
    {
        moveTowardHut = false;
        target = PathToVillage.pointsTowardVillage[0];
        StartCoroutine("Move");
    }
    public void StartMoveCoroutineToHut()
    {
        moveTowardHut = true;
        target = PathToHut.pointsTowardHut[0];
        StartCoroutine("Move");
    }

    public IEnumerator Move()
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
                    FlipSprite();
                    anim.SetFloat("moveSpeed", speed * animSpeed);
                    transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World);
                }
            }

            else
            {
                FlipSprite();
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
        Transform[] eitherPath;

        if (moveTowardHut)
            eitherPath = PathToHut.pointsTowardHut;
        else
            eitherPath = PathToVillage.pointsTowardVillage;

        if (waypointIndex >= eitherPath.Length - 1)
        {
            anim.SetFloat("moveSpeed", 0);
            StopCoroutine("Move"); //must use string
            if (!moveTowardHut)
                Destroy(gameObject);
            return;
        }
        else
        {
            waypointIndex++;
            target = eitherPath[waypointIndex];
            //target = PathToHut.pointsTowardHut[waypointIndex];
        }
    }

    public void FlipSprite()
    {
        if (transform.position.x < target.position.x)
            transform.localScale = new Vector3(myScale.x * 1, myScale.y, myScale.z);

        if (transform.position.x > target.position.x)
            transform.localScale = new Vector3(myScale.x * -1, myScale.y, myScale.z);
    }
}
