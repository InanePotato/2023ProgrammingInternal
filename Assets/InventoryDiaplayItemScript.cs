using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryDiaplayItemScript : MonoBehaviour
{
    public Item item;

    /// <summary>
    /// Used to handle when an item in the inventory is clicked...
    /// calls on the method in the players inventory manager
    /// to change item being viewed in item details
    /// </summary>
    public void SelectItem()
    {
        // calls on invletory manager to change item view item
        InventoryManager.Instance.ChangeInventoryItemView(item);
    }
}
