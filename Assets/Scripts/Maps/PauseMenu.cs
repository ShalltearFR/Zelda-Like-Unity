using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject Panel;
    private bool isPauseEnable;
    private Animator animator;
    private PlayerMovement playermovement;
    private bool stopSpamAnimation = false;
  //  public PlayerMovement.PlayerState playerCurrentState;

    private void Start()
    {
        animator = GameObject.Find("Pause Menu").GetComponent<Animator>();
        playermovement = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
    }


    private void Update()
    {
        if (!stopSpamAnimation)
        {
            if (!isPauseEnable)
            {
                if (Input.GetButtonDown("Start"))
                { StartCoroutine(pauseOn()); }
            }
            else
            {
                if (Input.GetButtonDown("Start"))
                { StartCoroutine(pauseOff()); }
            }
        }
    }

    private IEnumerator pauseOn()
    {

         //   stopSpamAnimation = true;
            //        if (playermovement.currentState == PlayerMovement.PlayerState.walk)
            //       { playermovement.currentState = PlayerMovement.PlayerState.idle; }

            playermovement.currentState = PlayerMovement.PlayerState.interact;

            animator.SetBool("pauseOn", true);

            yield return new WaitForSeconds(0.55f);
            animator.SetBool("pauseOn", false);
            isPauseEnable = true;
       //     yield return new WaitForSeconds(5f);
          //  stopSpamAnimation = false;
    }

    private IEnumerator pauseOff()
    {
        animator.SetBool("pauseOff", true);
        
        yield return new WaitForSeconds(0.55f);
        animator.SetBool("pauseOff", false);
        playermovement.currentState = PlayerMovement.PlayerState.idle;

        isPauseEnable = false;
    }
}
