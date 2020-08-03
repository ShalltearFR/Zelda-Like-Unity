using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryHUD : MonoBehaviour
{
    private Animator animator;
    private bool stopSpamAnimation = false;
    private PlayerMovement playermovement;
    private string playerStateTemp;
    private SoundManagement soundManagement;
    private SaveManager saveManager;
    private Inventory inventory;

    public CanvasGroup BasicHUD;
    public bool isPause;
    

    void Start()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            animator = GetComponent<Animator>();
            playermovement = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
            soundManagement = GameObject.Find("Sound Manager").GetComponent<SoundManagement>();
        }
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            if (!stopSpamAnimation && !isPause)
            {
                if (Input.GetButtonDown("Start"))
                { inventory = GameObject.Find("Save Manager").GetComponent<SaveManager>().objects[4] as Inventory; StartCoroutine(InventoryOn()); }
            }

            if (!stopSpamAnimation && isPause)
            {
                if (Input.GetButtonDown("Start"))
                { StartCoroutine(InventoryOff()); }
            }
        }
    }

    private IEnumerator InventoryOn()
    {
        // Procedure d'animation d'ouverture du menu d'inventaire
        // Changement d'etat du joueur et des mobs
        stopSpamAnimation = true;
        isPause = true;

        BasicHUD.alpha = 0;

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
        animator.SetTrigger("open");

        initInventory();

        // play sound ouverture menu
        soundManagement.soundEffectSource[4].clip = Resources.Load<AudioClip>("Audio/SE/Menu Open");
        soundManagement.soundEffectSource[4].Play();

        // Mise en place du curseur
        GameObject startSelectedButton = GameObject.Find("Continue");
        EventSystem eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        yield return new WaitForSeconds(0.05f);
        eventSystem.SetSelectedGameObject(startSelectedButton);

        yield return new WaitForSeconds(0.75f);
        stopSpamAnimation = false;
    }

    IEnumerator InventoryOff()
    {
        stopSpamAnimation = true;
        // Procedure d'animation d'ouverture du menu de pause
        // Changement d'etat du joueur et des mobs
        EventSystem eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        eventSystem.SetSelectedGameObject(null);

        animator.SetTrigger("close");

        //Joue le son de fermeture
        soundManagement.soundEffectSource[4].clip = Resources.Load<AudioClip>("Audio/SE/Menu Close");
        soundManagement.soundEffectSource[4].Play();

        yield return new WaitForSeconds(0.35f);

        BasicHUD.alpha = 1;
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
        stopSpamAnimation = false;
    }

    private void initInventory()
    {
        //Rend les images invisible
        Image[] imagesChild = transform.GetChild(1).GetComponentsInChildren<Image>();
        int i;
        for (i = 0; i < transform.GetChild(1).childCount; i++)
        {
            Color imagesChildColor = imagesChild[i].color;
            imagesChildColor.a = 0;

            imagesChild[i].color = imagesChildColor;
        }

        // Désactive tous les boutons
        Button[] buttonChild = transform.GetChild(1).GetComponentsInChildren<Button>();
//         i = 0;
        for (i = 0; i < transform.GetChild(1).childCount; i++)
        {
            buttonChild[i].interactable = false;
        }

        if (inventory.itemsName.Contains("Boomerang"))
        {
            Color imagesChildColor = imagesChild[0].color;
            imagesChildColor.a = 1;
            transform.GetChild(1).GetChild(0).GetComponent<Image>().color = imagesChildColor;
            transform.GetChild(1).GetChild(0).GetComponent<Button>().interactable = true;
        }

        if (inventory.itemsName.Contains("Bow"))
        {
            Color imagesChildColor = imagesChild[1].color;
            imagesChildColor.a = 1;
            transform.GetChild(1).GetChild(1).GetComponent<Image>().color = imagesChildColor;
            transform.GetChild(1).GetChild(1).GetComponent<Button>().interactable = true;
        }










    }


}
