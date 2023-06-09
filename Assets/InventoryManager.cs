using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("Game Manager")]
    private GameManagerScript gameManagerScript;
    private KeyCode openInventoryKeybind;

    [Header("UI")]
    public GameObject txtUiCoins;
    public GameObject InventoryView;
    private GameObject InventoryContent;
    private GameObject InventoryContentInformation;
    public GameObject InventoryItemDisplay;
    private bool inventoryOpen = false;

    [Header("Inventory")]
    public List<Item> Items = new List<Item>();
    public List<Item> sortedItems = new List<Item>();
    public int coins = 0;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // get the game manager script
        gameManagerScript = GameManagerScript.Instance;
        // get the keybind for inventory view toggling
        openInventoryKeybind = gameManagerScript.ToggleInventoryKeybind;

        // set the inventory to hidden/closed by default
        InventoryView.SetActive(false);
        inventoryOpen = false;

        // get the content and content info game objects
        InventoryContent = InventoryView.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject;
        InventoryContentInformation = InventoryView.transform.GetChild(1).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        // if the correct key for toggling the inventory view is pressed
        if (Input.GetKeyDown(openInventoryKeybind))
        {
            // toggle the inventory with the opposite of what it currently is
            toggleInventory(!inventoryOpen);
        }
    }

    /// <summary>
    /// This method is used for opening and closing the inventory.
    /// This includes adding and removing the item display objects
    /// </summary>
    /// <param name="isOpen"></param>
    public void toggleInventory(bool isOpen)
    {
        // the following 3 bools are determined by whether the inventory is opening or closing (the isOpen bool)
        // pause/play the game
        gameManagerScript.gamePaused = isOpen;
        // set the inventory view to active/inactive
        InventoryView.SetActive(isOpen);
        // set the inventory open to open/closed
        inventoryOpen = isOpen;

        // IF the inventory is open
        if (isOpen)
        {
            sortedItems.Clear();

            int maxItemId = 0;
            foreach (Item item in Items)
            {
                if (item.inventoryId > maxItemId)
                {
                    maxItemId = item.inventoryId;
                }
            }

            for (int i = 0; i <= maxItemId; i++)
            {
                foreach (Item item in Items)
                {
                    if (item.inventoryId == i)
                    {
                        sortedItems.Add(item);
                    }
                }
            }

            // add items, ammounts, and icons to inventory display for each of the items in the item list
            foreach (var item in sortedItems)
            {
                // spwan in a new item display
                GameObject newInventoryDisplayItem = Instantiate(InventoryItemDisplay, InventoryContent.transform);

                // get the ammount variable for the display
                var itemAmmount = newInventoryDisplayItem.transform.Find("Ammount").GetComponent<Text>();
                // get the icon variable for the display
                var itemIcon = newInventoryDisplayItem.transform.Find("Image").GetComponent<Image>();

                // set the ammount to the items ammount
                itemAmmount.text = item.ammount.ToString();
                // set the sprite to the items image
                itemIcon.sprite = item.sprite;

                if (item.type == Item.ItemType.key)
                {
                    itemIcon.transform.Rotate(Vector3.forward, 45);
                }
            }
        }
        else
        {
            // remove all items from inventory display
            foreach (Transform item in InventoryContent.transform)
            {
                // destroy the game object
                Destroy(item.gameObject);
            }
        }
    }

    /// <summary>
    /// When called, refreshes the text for coins ammount for the UI
    /// </summary>
    public void UpdateCoins()
    {
        // update the coins UI text with the correct value
        txtUiCoins.GetComponent<Text>().text = coins.ToString();
    }

    /// <summary>
    /// Creates a new item in the Items list with a default ammount
    /// </summary>
    /// <param name="item"></param>
    /// <param name="ammount"></param>
    public void CreateItem(Item item, int ammount)
    {
        // reset ammount back to 0
        item.ammount = 0;
        // add a new item to the list
        Items.Add(item);
        // give the item added a default ammount
        AddItemAmmount(item, ammount);
    }

    /// <summary>
    /// Removes a item from the Items list
    /// </summary>
    /// <param name="item"></param>
    public void RemoveItem(Item item)
    {
        // take the item off the list
        Items.Remove(item);
    }

    /// <summary>
    /// Adds a specified ammount to a specified item withen the Items list
    /// If the item dosn't exist, creates it
    /// </summary>
    /// <param name="item"></param>
    /// <param name="ammount"></param>
    public void AddItemAmmount(Item item, int ammount)
    {
        // IF the item to add is a currency
        if (item.type == Item.ItemType.currency)
        {
            // add the ammount
            coins += ammount;
            // update the UI coins text
            UpdateCoins();
        }
        else
        {
            // set item to not having been found by default
            bool itemFound = false;

            // search the list for the item
            foreach (Item i in Items)
            {
                // IF the correct item
                if (i == item)
                {
                    // set found to true
                    itemFound = true;
                    // add the ammount to the items ammount
                    i.ammount += ammount;
                }
            }

            // IF the item hasn't been found
            if (!itemFound)
            {
                // create the item
                CreateItem(item, ammount);
            }
        }
    }

    /// <summary>
    /// Subtracts a specified ammount to a specified item withen the Items list.
    /// If the item dosn't exist, creates it
    /// </summary>
    /// <param name="item"></param>
    /// <param name="ammount"></param>
    public void SubtractItemAmmount(Item item, int ammount)
    {
        // IF the item is a currency
        if (item.type == Item.ItemType.currency)
        {
            // subtract the ammount
            coins -= ammount;
            // update the UI coins ammount text
            UpdateCoins();
        }
        else
        {
            // set item not found be default
            bool itemFound = false;

            // search the items list
            foreach (Item i in Items)
            {
                // IF the item is found
                if (i == item)
                {
                    // set found to true
                    itemFound = true;
                    // subtract the ammount
                    i.ammount -= ammount;
                    
                    // IF the new ammount is negitive
                    if (i.ammount < 0)
                    {
                        // set the ammount to 0
                        i.ammount = 0;
                    }
                }
            }

            // if the item wasn't found
            if (!itemFound)
            {
                // create the item with the default value of 0
                CreateItem(item, 0);
            }
        }
    }

    /// <summary>
    /// Returns the details about an item in the Items list
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public Item GetItemDetails(Item item)
    {
        // search the list of items
        foreach (Item i in Items)
        {
            // IF found the item
            if (i == item)
            {
                // return the item found / it's details
                return i;
            }
        }

        // item not found so return null
        return null;
    }
}