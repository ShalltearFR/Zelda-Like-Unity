﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TreasureChest : Interactable
{
    public enum TypeOfItem
    {
        Key,
        Sword,
        Bomb,
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
    public int numberOfChest;

    private void Start()
    {
        anim = GetComponent<Animator>();
        playerMovement = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
        audioSource = GameObject.FindWithTag("SoundManager").GetComponent<SoundManagement>().soundEffectSource[0];

        BoolArrayValue chestBool = GameObject.Find("Save Manager").GetComponent<SaveManager>().objects[9] as BoolArrayValue;
        if (chestBool.initialValue[numberOfChest])
        {
            isOpen = true;
            anim.SetBool("opened", true);
        }

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

        // Sauvegarde le coffre indiquant qu'il a deja été ouvert
        BoolArrayValue chestBool = GameObject.Find("Save Manager").GetComponent<SaveManager>().objects[9] as BoolArrayValue;
        chestBool.initialValue[numberOfChest] = true;
        GameObject.Find("Save Manager").GetComponent<SaveManager>().objects[9] = chestBool;

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

        if (typeOfItem == TypeOfItem.Bow) { playerInventory.arrow = 30; GameObject.Find("Arrow HUD").GetComponent<ArrowTextManager>().UpdateArrowCount(); }
        if (typeOfItem == TypeOfItem.Bomb) { playerInventory.bomb = 10; GameObject.Find("Bomb HUD").GetComponent<BombTextManager>().UpdateBombCount(); }
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
