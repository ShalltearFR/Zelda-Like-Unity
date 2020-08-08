using UnityEngine;
using TMPro;
using System.IO;

public class ArrowTextManager : MonoBehaviour
{
    public Inventory playerInventory;
    public TextMeshProUGUI arrowDisplay;
    private SaveManager saveManager;

    private void Start()
    {
        saveManager = GameObject.FindWithTag("SaveManager").GetComponent<SaveManager>();
        DefaultArrow();
        UpdateArrowCount();
    }

    private void DefaultArrow()
    {
        // Si le fichier de sauvegarde n'existe pas, indique les flèches à 0 (n'est normalement plus necessaire)
        if (!File.Exists(saveManager.dataPath + "/4.data"))
        {
            playerInventory.arrow = 0;
            arrowDisplay.text = "00";
        }
    }

    public void UpdateArrowCount()
    {
        // Met à jour le nombre de rubis dans L'HUD
        if (playerInventory.arrow <= 9)
        {
            arrowDisplay.text = "0" + playerInventory.arrow;
        }
        if (playerInventory.arrow >= 10 && playerInventory.arrow <= 99)
        {
            arrowDisplay.text = "" + playerInventory.arrow;
        }

        if (playerInventory.arrow >= 100)
        {
            arrowDisplay.text = "99";
            playerInventory.arrow = 99;
        }
    }
}
