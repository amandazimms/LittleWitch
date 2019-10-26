using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CauldronBubbleRandomly : MonoBehaviour
{
    public Animator bubble1Anim;
    public Animator bubble2Anim;
    public Animator bubble3Anim;

    private void Awake()
    {
        bubble1Anim.speed = Random.Range(.8f, 1.2f);
        bubble2Anim.speed = Random.Range(.8f, 1.2f);
        bubble3Anim.speed = Random.Range(.8f, 1.2f);

        StartCoroutine(DoBubble(bubble1Anim));
        StartCoroutine(DoBubble(bubble2Anim));
        StartCoroutine(DoBubble(bubble3Anim));
    }

    IEnumerator DoBubble(Animator anim)
    {
        TransformVariance bubbleTV = anim.gameObject.GetComponent<TransformVariance>();
        bubbleTV.transform.localScale = new Vector3(.75f, .75f, .75f);
        bubbleTV.Awake();

        yield return new WaitForSeconds(Random.Range(1, 3));
        anim.SetTrigger("Bubble");

        yield return new WaitForSeconds(2f);
        StartCoroutine(DoBubble(anim));
    }
}
