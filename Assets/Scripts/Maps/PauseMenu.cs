﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    private GameObject Panel;
    public bool isPause;
    private Animator animator;
    private PlayerMovement playermovement;
    private bool stopSpamAnimation = false;
    //  public PlayerMovement.PlayerState playerCurrentState;
    public string playerStateTemp;

    private void Start()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            Panel = GameObject.FindWithTag("PauseMenu");
            animator = GameObject.FindWithTag("PauseMenu").GetComponent<Animator>();
            playermovement = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
        }
    }


    private void Update()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            if (!stopSpamAnimation)
            {
                if (!isPause)
                {
                    if (Input.GetButtonDown("Start"))
                    { StartCoroutine(pauseOn()); }
                }
            }
        }
    }

    public void ContinueButton()
    {
        StartCoroutine(pauseOff());
    }

    public void ExitButton()
    {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }

    private IEnumerator pauseOn()
    {
        stopSpamAnimation = true;
        isPause = true;
        if (playermovement.currentState == PlayerMovement.PlayerState.walk || playermovement.currentState == PlayerMovement.PlayerState.idle)
        {
            Animator playerAnimator = GameObject.FindWithTag("Player").GetComponent<Animator>();
            playerAnimator.SetBool("moving", false);
            playerStateTemp = "walk";
        }

        if (playermovement.currentState == PlayerMovement.PlayerState.takeObject)
        {
            Animator playerAnimator = GameObject.FindWithTag("Player").GetComponent<Animator>();
            playerAnimator.SetBool("WalkingObject", false);
            playerStateTemp = "walkingObject";
        }

        playermovement.currentState = PlayerMovement.PlayerState.interact;
        Panel.SetActive(true);
        animator.SetBool("pauseOn", true);

        yield return new WaitForSeconds(0.10f);
        animator.SetBool("pauseOn", false);
        yield return new WaitForSeconds(0.35f);
        
        GameObject startSelectedButton = GameObject.Find("Continue");
        EventSystem eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        yield return new WaitForSeconds(0.05f);
        eventSystem.SetSelectedGameObject(startSelectedButton);

        stopSpamAnimation = false;

    }

    private IEnumerator pauseOff()
    {
        EventSystem eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();

        eventSystem.SetSelectedGameObject(null);

        animator.SetBool("pauseOff", true);
        
        yield return new WaitForSeconds(0.10f);
        animator.SetBool("pauseOff", false);
        yield return new WaitForSeconds(0.35f);

        if (playerStateTemp == "walk")
        {
            Animator playerAnimator = GameObject.FindWithTag("Player").GetComponent<Animator>();
            playermovement.currentState = PlayerMovement.PlayerState.idle;
        }

        if (playerStateTemp == "walkingObject")
        {
            Animator playerAnimator = GameObject.FindWithTag("Player").GetComponent<Animator>();
            playermovement.currentState = PlayerMovement.PlayerState.takeObject;
        }
        isPause = false;
        Panel.SetActive(false);
    }
}