using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using Unity.VisualScripting;
using static GameManagerScript;

public class Inventory : MonoBehaviour
{
    [Header("Game Manager")]
    private GameObject gameManager;
    private GameManagerScript gameManagerScript;
    private KeyCode openInventoryKeybind;

    [Header("UI")]
    public GameObject txtUiCoins;
    public GameObject pnlInventory;
    public GameObject inventoryDisplayItemPrefab;
    private List<GameObject> inventoryDisplayItems = new List<GameObject>();
    private bool inventoryOpen = false;
    const float INVENTORY_DISPLAY_ITEM_WIDTH = 100;
    const float INVENTORY_DISPLAY_ITEM_HEIGHT = 130;
    const float INVENTORY_DISPLAY_ITEM_SPAWN_GAP = 0.5f;
    const float INVENTORY_DISPLAY_ITEM_BORDER = 0.1f;

    public enum ItemNames
    {
        Wodden_Key,
        Silver_Key,
        Gold_Key,
        Diamond_Key,
        Special_Key,
        Gold_Coin,
        Candy1_Red,
        Candy1_Blue,
        Candy1_Green,
        Candy1_Purple,
        Candy2_Red,
        Candy2_Blue,
        Candy2_Green,
        Candy2_Purple
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
    public Sprite goldCoinSprite;
    public Sprite candy1RedSprite;
    public Sprite candy1BlueSprite;
    public Sprite candy1GreenSprite;
    public Sprite candy1PurpleSprite;
    public Sprite candy2RedSprite;
    public Sprite candy2BlueSprite;
    public Sprite candy2GreenSprite;
    public Sprite candy2PurpleSprite;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindGameObjectsWithTag("GameController").First();
        gameManagerScript = gameManager.GetComponent<GameManagerScript>();
        openInventoryKeybind = gameManagerScript.ToggleInventoryKeybind;

        createInventoryItem(ItemNames.Wodden_Key, woddenKeySprite, false, false);
        createInventoryItem(ItemNames.Silver_Key, silverKeySprite, false, false);
        createInventoryItem(ItemNames.Gold_Key, goldKeySprite, false, false);
        createInventoryItem(ItemNames.Diamond_Key, diamondKeySprite, false, false);
        createInventoryItem(ItemNames.Special_Key, specialKeySprite, false, false);
        createInventoryItem(ItemNames.Gold_Coin, goldCoinSprite, false, false);
        createInventoryItem(ItemNames.Candy1_Red, candy1RedSprite, false, false);
        createInventoryItem(ItemNames.Candy1_Blue, candy1BlueSprite, false, false);
        createInventoryItem(ItemNames.Candy1_Green, candy1GreenSprite, false, false);
        createInventoryItem(ItemNames.Candy1_Purple, candy1PurpleSprite, false, false);
        createInventoryItem(ItemNames.Candy2_Red, candy2RedSprite, false, false);
        createInventoryItem(ItemNames.Candy2_Blue, candy2BlueSprite, false, false);
        createInventoryItem(ItemNames.Candy2_Green, candy2GreenSprite, false, false);
        createInventoryItem(ItemNames.Candy2_Purple, candy2PurpleSprite, false, false);

        //updateInventoryItemAmmount(ItemNames.Wodden_Key, 2);
        updateInventoryItemAmmount(ItemNames.Silver_Key, 4);
        updateInventoryItemAmmount(ItemNames.Gold_Key, 2);

        txtUiCoins.GetComponent<Text>().text = getItemAmmount(ItemNames.Gold_Coin).ToString();
        pnlInventory.SetActive(false);
        inventoryOpen = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(openInventoryKeybind))
        {
            toggleInventory(!inventoryOpen);
        }
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
                if (Name == ItemNames.Gold_Coin)
                {
                    txtUiCoins.GetComponent<Text>().text = getItemAmmount(ItemNames.Gold_Coin).ToString();
                }
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

    public int getItemAmmount(ItemNames name)
    {
        int itemAmmount = 0;

        foreach (InventoryItem item in inventoryItems)
        {
            if (item.name == name)
            {
                itemAmmount += item.ammount;
            }
        }

        return itemAmmount;
    }

    public void toggleInventory(bool isOpen)
    {
        gameManagerScript.gamePaused = isOpen;
        pnlInventory.SetActive(isOpen);
        inventoryOpen = isOpen;

        if (isOpen)
        {
            inventoryDisplayItems.Clear();

            float pnlInventoryWidth = pnlInventory.GetComponent<RectTransform>().rect.width;
            float pnlInventoryHeight = pnlInventory.GetComponent<RectTransform>().rect.height;

            float startX = -(pnlInventoryWidth / 2) + (INVENTORY_DISPLAY_ITEM_BORDER + (INVENTORY_DISPLAY_ITEM_WIDTH / 2));
            float startY = (pnlInventoryHeight / 2) - (INVENTORY_DISPLAY_ITEM_BORDER + (INVENTORY_DISPLAY_ITEM_HEIGHT / 2));

            float spawnX = startX;
            float spawnY = startY;
            
            int numOfItemsPerRow = (int)(pnlInventoryWidth / (INVENTORY_DISPLAY_ITEM_WIDTH + INVENTORY_DISPLAY_ITEM_SPAWN_GAP));

            int index = 0;
            foreach (InventoryItem item in inventoryItems)
            {
                if (item.name != ItemNames.Gold_Coin)
                {
                    if (index >= numOfItemsPerRow)
                    {
                        spawnX = startX;
                        spawnY -= (INVENTORY_DISPLAY_ITEM_HEIGHT + INVENTORY_DISPLAY_ITEM_SPAWN_GAP);
                    }

                    GameObject newItem = Instantiate(inventoryDisplayItemPrefab);
                    newItem.transform.SetParent(pnlInventory.transform);
                    newItem.GetComponent<RectTransform>().localPosition = new Vector3(spawnX, spawnY, newItem.GetComponent<RectTransform>().localPosition.z);
                    newItem.GetComponentsInChildren<Image>()[1].sprite = item.sprite;
                    newItem.GetComponentInChildren<Text>().text = item.ammount.ToString();

                    if (isKey(item))
                    {

                        newItem.GetComponentsInChildren<Transform>()[2].Rotate(Vector3.forward, 45);
                    }

                    inventoryDisplayItems.Add(newItem);

                    spawnX += (INVENTORY_DISPLAY_ITEM_WIDTH + INVENTORY_DISPLAY_ITEM_SPAWN_GAP);
                }
            }
        }
        else
        {
            foreach (GameObject item in inventoryDisplayItems)
            {
                Destroy(item);
            }

            inventoryDisplayItems.Clear();
        }
        
    }

    public bool isKey(InventoryItem item)
    {
        if (item.name == ItemNames.Wodden_Key || item.name == ItemNames.Silver_Key || item.name == ItemNames.Gold_Key || item.name == ItemNames.Diamond_Key || item.name == ItemNames.Special_Key)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
