using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RubisTextManager : MonoBehaviour
{
    public Inventory playerInventory;
    public TextMeshProUGUI rubisDisplay;

    private void Start()
    {
        UpdateRubisCount();
        playerInventory.rubisTemp = 0;
    }

    public void UpdateRubisCount()
    {
        if (playerInventory.rubis <= 9)
        {
            rubisDisplay.text = "00" + playerInventory.rubis;
        }
        if (playerInventory.rubis >= 10 && playerInventory.rubis <= 99)
        {
            rubisDisplay.text = "0" + playerInventory.rubis;
        }

        if (playerInventory.rubis >= 100 && playerInventory.rubis <= 999)
        {
            rubisDisplay.text = "" + playerInventory.rubis;
        }

        if (playerInventory.rubis >= 1000)
        {
            rubisDisplay.text = "999";
            playerInventory.rubis = 999;
        }
    }
}
