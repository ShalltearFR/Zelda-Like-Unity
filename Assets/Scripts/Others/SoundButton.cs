using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class SoundButton : MonoBehaviour
{
    private AudioSource[] audiosource = new AudioSource[2];
    private SoundManagement soundManagement;
    private SaveManager saveManager;
    private Image itemDescription;
    private InventoryHUD inventoryHUD;

    private void Start()
    {
        soundManagement = GameObject.FindWithTag("SoundManager").GetComponent<SoundManagement>();
        audiosource[0] = soundManagement.soundEffectSource[0];
        audiosource[1] = soundManagement.soundEffectSource[1];
        saveManager = GameObject.Find("Save Manager").GetComponent<SaveManager>();
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            itemDescription = GameObject.Find("Item Description").GetComponent<Image>();
            inventoryHUD = GameObject.Find("Inventory HUD").GetComponent<InventoryHUD>();
        }
    }

    private void Cursor()
    {
        // Si le cursor est sur le bouton
        audiosource[0].clip = Resources.Load<AudioClip>("Audio/SE/Cursor");
        audiosource[0].Play();
    }

    private void Select()
    {
        // Si la touche de confirmation est appuyé
        audiosource[1].clip = Resources.Load<AudioClip>("Audio/SE/Message Finish");
        audiosource[1].Play();
    }

    private void CursorSlot()
    {
        // Permet de detecter le type de slot selon la position du curseur
        // Pour le menu principal
        int slot = 0;
        if (gameObject.name == "Slot Button First") { slot = 1; }
        if (gameObject.name == "Slot Button Second") { slot = 2; }
        if (gameObject.name == "Slot Button Third") { slot = 3; }

        saveManager.cursorSlot = slot;
    }

    private void CursorItem()
    {
        // Permet de detecter le type d'item selon la position du curseur
        // Pour le menu d'inventaire

        // Indique dans le save manager le type d'item equipé
        StringValue cursorItem = saveManager.selectedItem as StringValue;


        if (gameObject.name == "Boomerang") { cursorItem.RuntimeValue = "Boomerang"; }
        if (gameObject.name == "Bow") { cursorItem.RuntimeValue = "Bow"; }
        if (gameObject.name == "Bomb") { cursorItem.RuntimeValue = "Bomb"; }

        saveManager.selectedItem = cursorItem;

        // Affiche la description de l'item dans le menu d'inventaire
        if (cursorItem.RuntimeValue == "Boomerang") { itemDescription.sprite = saveManager.itemDescriptionImage[0]; }
        if (cursorItem.RuntimeValue == "Bow") { itemDescription.sprite = saveManager.itemDescriptionImage[1]; }
        if (cursorItem.RuntimeValue == "Bomb") { itemDescription.sprite = saveManager.itemDescriptionImage[2]; }

        inventoryHUD.InitItemBar();
    }
}
