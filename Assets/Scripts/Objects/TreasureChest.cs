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
        audioSource = GameObject.FindWithTag("SoundManager").GetComponent<SoundManagement>().soundEffectSource[0];
    }
    private void Update()
    {
        if (Input.GetButtonDown("Interract") && playerInRange)
        {
            if (!isOpen)
            {
                // Ouvre le coffre
                OpenChest();
            } else
            {
                // Coffre est deja ouvert
                ChestAlreadyOpen();
            }
        }
    }

    public void OpenChest()
    {
        // Ouvre le contenu du coffre et l'ajoute dans l'inventaire du personnage
        dialogBox.SetActive(true);
        dialogText.text = contents.itemDescription;
        playerInventory.AddItem(contents,typeOfItem);

        if (typeOfItem == TypeOfItem.FullHeart || typeOfItem == TypeOfItem.Grappin || typeOfItem == TypeOfItem.Boomerang || typeOfItem == TypeOfItem.Bow || typeOfItem == TypeOfItem.Sword)
        {
            audioSource.clip = Resources.Load<AudioClip>("Audio/SE/Treasure Chest");
            audioSource.Play();
        }

        playerMovement.RaiseItem(typeOfItem);
        playerInventory.currentItem = contents;
        
        contextOn.Raise();
        isOpen = true;
        anim.SetBool("opened", true);
        contextOff.Raise();
    }

    public void ChestAlreadyOpen()
    {
        // Le coffre est deja ouvert donc desactive l'interraction avec le coffre
        dialogBox.SetActive(false);
        playerMovement.RaiseItem(typeOfItem);
        enable = false;
        playerInRange = false;
        contextOff.Raise();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger && !isOpen)
        {
            // Affiche le "!" quand le joueur est à porté du coffre
            contextOn.Raise();
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger && !isOpen)
        {
            // Retire le "!" quand le joueur est hors de portée du coffre
            contextOff.Raise();
            playerInRange = false;
        }
    }

}
