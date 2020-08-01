using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
[System.Serializable]

public class Inventory : ScriptableObject
{
    public Item currentItem;
    public List<Item> items = new List<Item>();
    public List<string> itemsName = new List<string>();
    public int numberofKeys;
    public int rubis;
    public int rubisTemp;

    public void AddItem(Item itemToAdd, TreasureChest.TypeOfItem typeOfItem)
    {
        // Est ce que l'item est une clé ?
        if (itemToAdd.isKey)
        {
            numberofKeys++;
        } else
        {
        // Rajoute l'item dans l'inventaire
            if (!items.Contains(itemToAdd))
            {
                itemsName.Add(typeOfItem.ToString());
                items.Add(itemToAdd);
            }
        }
    }
}
