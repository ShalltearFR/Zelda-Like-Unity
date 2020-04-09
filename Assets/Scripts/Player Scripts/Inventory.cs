using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
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
        // is the item a key ?
        if (itemToAdd.isKey)
        {
            numberofKeys++;
        } else
        {
            if (!items.Contains(itemToAdd))
            {
                itemsName.Add(typeOfItem.ToString());
                items.Add(itemToAdd);
            }
        }
    }
}
