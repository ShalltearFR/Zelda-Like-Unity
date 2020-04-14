﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public class RubisTextManager : MonoBehaviour
{
    public Inventory playerInventory;
    public TextMeshProUGUI rubisDisplay;
    private SaveManager saveManager;

    private void Start()
    {
        saveManager = GameObject.FindWithTag("SaveManager").GetComponent<SaveManager>();
        DefaultRubis();

        UpdateRubisCount();
        playerInventory.rubisTemp = 0;
    }

    private void DefaultRubis()
    {
        if (!File.Exists(saveManager.dataPath + "/4.dat"))
        {
            playerInventory.rubis = 0;
            rubisDisplay.text = "000";
        }
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
