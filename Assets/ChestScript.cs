using System;
using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class ChestScript : MonoBehaviour
{
    // Declare Class scope variable for game manager reference
    [Header("Game Manager")]
    private GameManagerScript gameManagerScript;

    // Declare Class scope variable for dropped item prefab
    [Header("Dropped Item Prefab")]
    private GameObject droppedItem;

    // Declare Class scope variables chests information
    [Header("Chest Information")]
    public ChestType chestType;
    public bool chestOpen = false;
    private SpriteRenderer chestSpriteRenderer;
    private Transform chestTransform;
    public float lootSpawnOffsetY;
    public float lootSpawnOffsetX;
    public float lootSpawnDistanceBelow = 0.2f;
    public Sprite openSprite;
    public Sprite closedSprite;

    // Declare Class scope variables loot information
    [Header("Required Loot")]
    public List<LootItem> loot = new List<LootItem>();
    public LootTable lootTable;
    public int minLootTableDrops;
    public int maxLootTableDrops;

    // Declare Serializable structure for loot items
    [Serializable]
    public struct LootItem
    {
        public Item item;
        public int minAmmount;
        public int maxAmmount;
    }

    // Declare public enum for chest types
    public enum ChestType
    {
        SilverChest,
        GoldChest
    }

    // Start is called before the first frame update
    void Start()
    {
        // Get and set reference to game manager script
        gameManagerScript = GameManagerScript.Instance;

        // Get and set references to objects renderer and transform components
        chestSpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        chestTransform = gameObject.GetComponent<Transform>();

        // Set reference to dropped item prefab
        droppedItem = gameManagerScript.droppedItemPrefab;
    }

    /// <summary>
    /// Handles any opening and closing of the chest
    /// </summary>
    /// <param name="isOpen"></param>
    public void openChest(bool isOpen)
    {
        // Set chest open state bool to given value
        chestOpen = isOpen;

        // IF chest is now open
        if (chestOpen)
        {
            // Change to open sprite
            chestSpriteRenderer.sprite = openSprite;

            // sorts out specified loot
            // FOREACH loot item in the loot list
            foreach (LootItem lootItem in loot)
            {
                // Set a random amount based on items min & max values
                int ammount = UnityEngine.Random.Range(lootItem.minAmmount, lootItem.maxAmmount);
                // Call on the DropItem method with item and amount
                DropItem(lootItem.item, ammount);
            }

            // sorts out loot table loot
            // IF chest can drop items from a loot table
            if (lootTable != null && maxLootTableDrops > 0)
            {
                // prepare lists for selection based on rarity
                // Declare list for loot table ID’s based on rarity
                List<int> LootTableItemIDS = new List<int>();
                // FOR amount of items in loot table
                for (int id = 0; id < lootTable.lootTable.Count(); id++)
                {
                    // FOR rarity value of items in loot table
                    for (int i = 0; i < lootTable.lootTable[id].rarity; i++)
                    {
                        // Add item loot table ID to list
                        LootTableItemIDS.Add(id);
                    }
                }

                // Set a random number of drops based on min and max values
                int randomNumDrops = UnityEngine.Random.Range(minLootTableDrops, maxLootTableDrops);
                // FOR number of drops
                for (int i = 0; i < randomNumDrops; i++)
                {
                    // Pick a random ID from the ID’s list
                    int randomID = LootTableItemIDS[UnityEngine.Random.Range(0, LootTableItemIDS.Count() - 1)];

                    // Set a random amount of the item to drop based on min and max values
                    int ammount = UnityEngine.Random.Range(lootTable.lootTable[randomID].minAmmountPerDrop, lootTable.lootTable[randomID].maxAmmountPerDrop);
                    // Set item to be dropped
                    Item item = lootTable.lootTable[randomID].item;

                    // Call on the DropItem method with item and amount
                    DropItem(item, ammount);
                }
            }
        }
        else
        {
            // Chest not closed
            // Set chest sprite to closed
            chestSpriteRenderer.sprite = closedSprite;
        }
    }

    /// <summary>
    /// Handles the dropping of an item based on a given item and ammount
    /// </summary>
    /// <param name="item"></param>
    /// <param name="ammount"></param>
    private void DropItem(Item item, int ammount)
    {
        // Spawn in a new dropped item
        GameObject newItemDrop = Instantiate(droppedItem);

        // Set an X & Y location based on object position, offset, and random scatter
        float X = chestTransform.position.x + UnityEngine.Random.Range(-lootSpawnOffsetX, lootSpawnOffsetX);
        float Y = chestTransform.position.y + UnityEngine.Random.Range(-lootSpawnOffsetY, lootSpawnOffsetY) - lootSpawnDistanceBelow;
        newItemDrop.transform.position = new Vector2(X, Y);

        // Set a random rotation for the dropped item
        newItemDrop.transform.Rotate(Vector3.forward, UnityEngine.Random.Range(-80, 80));

        // Assign the ItemPickup Script of the dropped item an item and amount
        newItemDrop.GetComponent<ItemPickup>().item = item;
        newItemDrop.GetComponent<ItemPickup>().ammount = ammount;
    }
}
