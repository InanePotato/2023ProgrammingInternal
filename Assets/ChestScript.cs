using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Progress;

public class ChestScript : MonoBehaviour
{
    [Header("Game Manager")]
    private GameManagerScript gameManagerScript;

    [Header("Dropped Item Prefab")]
    private GameObject droppedItem;

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

    [Header("Required Loot")]
    public List<LootItem> loot = new List<LootItem>();
    public LootTable lootTable;
    public int minLootTableDrops;
    public int maxLootTableDrops;

    [Serializable]
    public struct LootItem
    {
        public Item item;
        public int minAmmount;
        public int maxAmmount;
    }

    public enum ChestType
    {
        SilverChest,
        GoldChest
    }

    // Start is called before the first frame update
    void Start()
    {
        // gets the game manager script
        gameManagerScript = GameManagerScript.Instance;

        // gets this objects renderer and transform components
        chestSpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        chestTransform = gameObject.GetComponent<Transform>();

        droppedItem = gameManagerScript.droppedItemPrefab;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void openChest(bool isOpen)
    {
        // sets the chest to be either open or closed
        chestOpen = isOpen;

        // IF the chest is open
        if (chestOpen)
        {
            // sets the approprite sprite
            chestSpriteRenderer.sprite = openSprite;

            // release/spawn in loot
            foreach (LootItem lootItem in loot)
            {
                int ammount = UnityEngine.Random.Range(lootItem.minAmmount, lootItem.maxAmmount);
                DropItem(lootItem.item, ammount);
            }

            // release loot table loot
            if (lootTable != null && maxLootTableDrops > 0)
            {
                // prepare table for item selection based on 'rarity'
                List<int> LootTableItemIDS = new List<int>();
                for (int id = 0; id < lootTable.lootTable.Count(); id++)
                {
                    for (int i = 0; i < lootTable.lootTable[id].rarity; i++)
                    {
                        LootTableItemIDS.Add(id);
                    }
                }

                // drop loot table loot
                int randomNumDrops = UnityEngine.Random.Range(minLootTableDrops, maxLootTableDrops);
                for (int i = 0; i < randomNumDrops; i++)
                {
                    //get a random id to drop
                    int randomID = LootTableItemIDS[UnityEngine.Random.Range(0, LootTableItemIDS.Count() - 1)];

                    // get ammount
                    int ammount = UnityEngine.Random.Range(lootTable.lootTable[randomID].minAmmountPerDrop, lootTable.lootTable[randomID].maxAmmountPerDrop);
                    // get Item
                    Item item = lootTable.lootTable[randomID].item;

                    DropItem(item, ammount);
                }
            }
        }
        else
        {
            chestSpriteRenderer.sprite = closedSprite;
        }
    }

    private void DropItem(Item item, int ammount)
    {
        GameObject newItemDrop = Instantiate(droppedItem);

        float X = chestTransform.position.x + UnityEngine.Random.Range(-lootSpawnOffsetX, lootSpawnOffsetX);
        float Y = chestTransform.position.y + UnityEngine.Random.Range(-lootSpawnOffsetY, lootSpawnOffsetY) - lootSpawnDistanceBelow;
        newItemDrop.transform.position = new Vector2(X, Y);

        newItemDrop.transform.Rotate(Vector3.forward, UnityEngine.Random.Range(-80, 80));

        newItemDrop.GetComponent<ItemPickup>().item = item;
        newItemDrop.GetComponent<ItemPickup>().ammount = ammount;
    }
}
