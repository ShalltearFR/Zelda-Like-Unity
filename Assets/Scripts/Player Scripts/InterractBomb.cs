using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterractBomb : MonoBehaviour
{
    private BoxCollider2D boxcollider;
    private Animator playerAnimator;
    private bool isEnter;
    private bool isLifting;

    // Start is called before the first frame update
    void Start()
    {
        boxcollider = GetComponent<BoxCollider2D>();
        playerAnimator = GameObject.FindWithTag("Player").GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if (Input.GetButtonDown("Interract") & isEnter & !isLifting)
        {
            isLifting = true;
            gameObject.transform.parent.transform.SetParent(GameObject.FindWithTag("TakeObject").transform);
            StartCoroutine(LiftingAnimation());
        }
    }

    IEnumerator LiftingAnimation()
    {
        GameObject.FindWithTag("Player").GetComponent<PlayerMovement>().speed = 0;
        gameObject.transform.parent.transform.localPosition = Vector3.zero;
        playerAnimator.SetBool("LiftingBomb", true);
        yield return null;
        playerAnimator.SetBool("LiftingBomb", false);
        yield return new WaitForSeconds(0.35f);
        GameObject.FindWithTag("Player").GetComponent<PlayerMovement>().currentState = PlayerMovement.PlayerState.takeObject;
        GameObject.FindWithTag("Player").GetComponent<PlayerMovement>().speed = 3;
        playerAnimator.SetBool("TakingBomb", true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isEnter)
        {
            if (other.CompareTag("Player"))
            {
                isEnter = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (isEnter)
        {
            if (other.CompareTag("Player"))
            {
                isEnter = false;
            }
        }
    }
}
