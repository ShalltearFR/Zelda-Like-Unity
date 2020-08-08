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
    private Image itemDescriptionImage;
    private EventSystem eventSystem;
    private Color color;

    public CanvasGroup BasicHUD;
    public bool isInInventoryHUD;
    

    void Start()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            animator = GetComponent<Animator>();
            playermovement = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
            soundManagement = GameObject.Find("Sound Manager").GetComponent<SoundManagement>();
            saveManager = GameObject.Find("Save Manager").GetComponent<SaveManager>();
            itemDescriptionImage = GameObject.Find("Item Description").GetComponent<Image>();
            eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
            color = GameObject.Find("Item HUD").GetComponent<Image>().color;

            Invoke("InitItemBar",1f);
        }
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            if (playermovement.currentState != PlayerMovement.PlayerState.interact)
            {
                if (!stopSpamAnimation && !isInInventoryHUD)
                {
                    if (Input.GetButtonDown("Start"))
                    { inventory = GameObject.Find("Save Manager").GetComponent<SaveManager>().objects[4] as Inventory; StartCoroutine(InventoryOn()); }
                }
            }

            if (!stopSpamAnimation && isInInventoryHUD)
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
        isInInventoryHUD = true;

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

        StartCoroutine(initInventory());

        // play sound ouverture menu
        soundManagement.soundEffectSource[4].clip = Resources.Load<AudioClip>("Audio/SE/Menu Open");
        soundManagement.soundEffectSource[4].Play();


        
        yield return new WaitForSeconds(0.75f);
        stopSpamAnimation = false;
        
    }

    public void InitItemBar()
    {
        if (saveManager.selectedItem.RuntimeValue != "")
        {
            color.a = 1;
            GameObject.Find("Item HUD").GetComponent<Image>().color = color;
            GameObject.Find("Item HUD").GetComponent<Image>().sprite = GameObject.Find(saveManager.selectedItem.RuntimeValue).GetComponent<Image>().sprite;
        } else
        {
            color.a = 0;
            GameObject.Find("Item HUD").GetComponent<Image>().color = color;
        }
    }


    IEnumerator InventoryOff()
    {
        stopSpamAnimation = true;
        isInInventoryHUD = false;

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

        stopSpamAnimation = false;
    }

    private IEnumerator initInventory()
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

        // Detection d'inventaire et activation des boutons item
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

        if (inventory.itemsName.Contains("Bomb"))
        {
            Color imagesChildColor = imagesChild[2].color;
            imagesChildColor.a = 1;
            transform.GetChild(1).GetChild(2).GetComponent<Image>().color = imagesChildColor;
            transform.GetChild(1).GetChild(2).GetComponent<Button>().interactable = true;
        }

        yield return null;

        if (saveManager.selectedItem.RuntimeValue == "")
        {
            bool isCursorSelected = false;
            //Initialise le curseur sur le 1er item
            // Boomerang
            if (!isCursorSelected && transform.GetChild(1).GetChild(0).GetComponent<Image>().color.a == 1)
            {
                StartCoroutine(SetCursor(transform.GetChild(1).GetChild(0).gameObject));
                isCursorSelected = true;
            }

            // Arrow
            if (!isCursorSelected && transform.GetChild(1).GetChild(1).GetComponent<Image>().color.a == 1)
            {
                StartCoroutine(SetCursor(transform.GetChild(1).GetChild(1).gameObject));
                isCursorSelected = true;
            }

            // Bomb
            if (!isCursorSelected && transform.GetChild(1).GetChild(2).GetComponent<Image>().color.a == 1)
            {
                StartCoroutine(SetCursor(transform.GetChild(1).GetChild(2).gameObject));
                isCursorSelected = true;
            }
            isCursorSelected = false;
        } else { StartCoroutine(SetCursor(GameObject.Find(saveManager.selectedItem.RuntimeValue))); } // Initialise le curseur sur l'item equipé

        yield return new WaitForSeconds(0.5f);

        // Si pas d'item dans l'inventaire
        // Alpha de l'item description à 0 sinon alpha à 1
        Color imagesColor = itemDescriptionImage.color;
        if (saveManager.selectedItem.RuntimeValue == "")
        {
            imagesColor.a = 0;
            itemDescriptionImage.color = imagesColor;
        }
        else
        {
            imagesColor.a = 1;
            itemDescriptionImage.color = imagesColor;
        }

        InitItemBar();
    }


    private IEnumerator SetCursor(GameObject button)
    {
        yield return new WaitForSeconds(0.05f);
        eventSystem.SetSelectedGameObject(button);
    }

}
