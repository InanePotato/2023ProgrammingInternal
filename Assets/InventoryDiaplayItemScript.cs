using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryDiaplayItemScript : MonoBehaviour
{
    public Item item;

    public void SelectItem()
    {
        InventoryManager.Instance.ChangeInventoryItemView(item);
    }
}
