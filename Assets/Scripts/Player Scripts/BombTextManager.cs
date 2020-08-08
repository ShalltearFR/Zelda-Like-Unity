using UnityEngine;
using TMPro;
using System.IO;

public class BombTextManager : MonoBehaviour
{
    public Inventory playerInventory;
    public TextMeshProUGUI bombDisplay;
    private SaveManager saveManager;

    private void Start()
    {
        saveManager = GameObject.FindWithTag("SaveManager").GetComponent<SaveManager>();
        DefaultBomb();
        UpdateBombCount();
    }

    private void DefaultBomb()
    {
        // Si le fichier de sauvegarde n'existe pas, indique les flèches à 0 (n'est normalement plus necessaire)
        if (!File.Exists(saveManager.dataPath + "/4.data"))
        {
            playerInventory.bomb = 0;
            bombDisplay.text = "00";
        }
    }

    public void UpdateBombCount()
    {
        // Met à jour le nombre de rubis dans L'HUD
        if (playerInventory.bomb <= 9)
        {
            bombDisplay.text = "0" + playerInventory.bomb;
        }
        if (playerInventory.bomb >= 10 && playerInventory.bomb <= 29)
        {
            bombDisplay.text = "" + playerInventory.bomb;
        }

        if (playerInventory.bomb >= 30)
        {
            bombDisplay.text = "30";
            playerInventory.bomb = 30;
        }
    }
}
