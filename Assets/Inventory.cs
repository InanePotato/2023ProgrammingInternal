using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public enum ItemNames
    {
        Wodden_Key,
        Silver_Key,
        Gold_Key,
        Diamond_Key,
        Special_Key
    }

    public class InventoryItem
    {
        public ItemNames name;
        public Sprite sprite;
        public int ammount;
        public bool equipable;
        public bool equiped;

        public InventoryItem(ItemNames Name, Sprite ItemSprite, int Ammount, bool Equipable, bool Equiped)
        {
            name = Name;
            sprite = ItemSprite;
            ammount = Ammount;
            equipable = Equipable;
            equiped = Equiped;
        }
    }

    public List<InventoryItem> inventoryItems = new List<InventoryItem>();

    [Header("Sprites")]
    public Sprite woddenKeySprite;
    public Sprite silverKeySprite;
    public Sprite goldKeySprite;
    public Sprite diamondKeySprite;
    public Sprite specialKeySprite;

    // Start is called before the first frame update
    void Start()
    {
        createInventoryItem(ItemNames.Wodden_Key, woddenKeySprite, false, false);
        createInventoryItem(ItemNames.Silver_Key, silverKeySprite, false, false);
        createInventoryItem(ItemNames.Gold_Key, goldKeySprite, false, false);
        createInventoryItem(ItemNames.Diamond_Key, diamondKeySprite, false, false);
        createInventoryItem(ItemNames.Special_Key, specialKeySprite, false, false);

        updateInventoryItemAmmount(ItemNames.Silver_Key, 2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void createInventoryItem(ItemNames Name, Sprite ItemSprite, bool Equipable, bool Equiped)
    {
        inventoryItems.Add(new InventoryItem(Name, ItemSprite, 0, Equipable, Equiped));
    }

    public void updateInventoryItemAmmount(ItemNames Name, int Ammount)
    {
        foreach (InventoryItem item in inventoryItems)
        {
            if (Name == item.name)
            {
                item.ammount += Ammount;
            }
        }
    }

    public bool checkHasItem(ItemNames name)
    {
        bool hasItem = false;
        
        foreach (InventoryItem item in inventoryItems)
        {
            if (item.name == name && item.ammount > 0)
            {
                hasItem = true;
            }
        }

        return hasItem;
    }
}
