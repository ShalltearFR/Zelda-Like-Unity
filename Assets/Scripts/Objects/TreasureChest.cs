using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TreasureChest : Interactable
{
    public enum TypeOfItem
    {
        Key,
        Sword,
        Bombe,
        Boomerang,
        Boussole,
        Map,
        FullHeart,
        Bow,
        QuarterHeart,
        Grappin
    }
    public TypeOfItem typeOfItem;

    public Item contents;
    public Inventory playerInventory;
    public bool isOpen;
    public Signal_Event raiseItem;
    public GameObject dialogBox;
    public Text dialogText;
    private Animator anim;
    private PlayerMovement playerMovement;
    private AudioSource audioSource;

    private void Start()
    {
        anim = GetComponent<Animator>();
        playerMovement = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
        audioSource = GameObject.FindWithTag("SoundManager").GetComponent<SoundManagement>().audioSource[0];
    }
    private void Update()
    {
        if (Input.GetButtonDown("Interract") && playerInRange)
        {
            if (!isOpen)
            {
                //Open Chest
                OpenChest();
            } else
            {
                // chest is allready open
                ChestAlreadyOpen();
            }
        }
    }

    public void animateChest()
    {
        playerMovement.RaiseItem(typeOfItem);
    }

    public void OpenChest()
    {
        // Dialog Window on
        dialogBox.SetActive(true);
        // Dialog text = content text
        dialogText.text = contents.itemDescription;
        // add content to the inventory
        playerInventory.AddItem(contents,typeOfItem);

        if (typeOfItem == TypeOfItem.FullHeart || typeOfItem == TypeOfItem.Grappin || typeOfItem == TypeOfItem.Boomerang || typeOfItem == TypeOfItem.Bow || typeOfItem == TypeOfItem.Sword)
        {
            audioSource.clip = Resources.Load<AudioClip>("Audio/SE/Treasure Chest");
            audioSource.Play();
        }
        playerInventory.currentItem = contents;
        // Raise the signal to the player to animate
        raiseItem.Raise();
        // raise the context clue
        contextOn.Raise();
        // set the chest to opened
        isOpen = true;
        anim.SetBool("opened", true);
        contextOff.Raise();
    }

    public void ChestAlreadyOpen()
    {
            // Dialog off
            dialogBox.SetActive(false);
        // set the current item to empty
        //    playerInventory.currentItem = null;
        // raise the signal to the player to stop animating
        
            raiseItem.Raise();
        enable = false;
        playerInRange = false;
        contextOff.Raise();

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger && !isOpen)
        {
            contextOn.Raise();
            playerInRange = true;

        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger && !isOpen)
        {
            contextOff.Raise();
            playerInRange = false;
        }
    }

}
