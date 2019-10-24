using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TransformVariance))] //because otherwise no one sets myScale
public class MoveAlongPath : MonoBehaviour
{
    public float speed = .5f;
    public float animSpeed = .9f;

    PeasantStats peasantStats;

    Animator anim;
    [HideInInspector] public WaveSpawner waveSpawner;
    public GameObject peasantInFrontOfMe;

    Transform target;
    int waypointIndex = 0;

    public bool moveTowardHut; //if false, moveTowardVillage

    public Vector3 myScale; //set by TransformVariance

    private void Start()
    {
        peasantStats = GetComponent<PeasantStats>();
        anim = GetComponent<Animator>();
        StartMoveCoroutineToHut();
    }

    public void StartMoveCoroutineToHut()
    {
        moveTowardHut = true;
        target = PathToHut.pointsTowardHut[0];
        StartCoroutine("Move");
    }

    public void StartMoveCoroutineToVillage()
    {
        moveTowardHut = false;
        target = PathToVillage.pointsTowardVillage[0];
        StartCoroutine("Move");
    }

    public void StopMoveCoroutine()
    {
        StopCoroutine("Move");
        anim.SetFloat("moveSpeed", 0);
    }

    public IEnumerator Move()
    {
        while (true)
        {
            bool aPeasantIsInFrontOfMe = false;
            Vector3 dir = target.position - transform.position; 

            if (moveTowardHut)
            {
                foreach (GameObject spawnedPeasant in waveSpawner.spawnedPeasants)
                {
                    if (spawnedPeasant != null && spawnedPeasant != gameObject && spawnedPeasant.transform.position.x > transform.position.x //first check to make sure there is a peasant there, and it's not myself, and it's in front of (towards hut from) me
                        && Vector3.Distance(spawnedPeasant.transform.position, transform.position) <= .5f) //if I'm really close to another peasant, stop walking so we form a queue.
                    {
                        aPeasantIsInFrontOfMe = true;
                        anim.SetFloat("moveSpeed", 0);
                    }
                }

                if (!aPeasantIsInFrontOfMe) //if there's no one in my way, walk.
                {
                    FlipSprite();
                    anim.SetFloat("moveSpeed", speed * animSpeed);
                    transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World);
                }
            }

            else //If i'm not on the way to the hut, I'm on the way home/to village, which means I'm the only one in my immediate area, so just get out of there without caring if there are other peasants nearby
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

        if (waypointIndex >= eitherPath.Length - 1) // if we reached the destination
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
