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
    const float LOOT_SPAWN_DISTANCE_BELOW = 0.2f;
    public Sprite openSprite;
    public Sprite closedSprite;

    [Header("Required Loot")]
    public List<Item> requiredLootItemObjects = new List<Item>();
    public List<int> requiredLootItemAmmounts = new List<int>();

    public class LootItem
    {
        public GameObject itemPrefab;
        public int ammount;

        public LootItem(GameObject ItemPrefab, int Ammount)
        {
            itemPrefab = ItemPrefab;
            ammount = Ammount;
        }
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
            int count = 0;
            foreach (Item item in requiredLootItemObjects)
            {
                GameObject newItemDrop = Instantiate(droppedItem);

                float X = chestTransform.position.x + Random.Range(-lootSpawnOffsetX, lootSpawnOffsetX);
                float Y = chestTransform.position.y + Random.Range(-lootSpawnOffsetY, lootSpawnOffsetY) - LOOT_SPAWN_DISTANCE_BELOW;
                newItemDrop.transform.position = new Vector2(X, Y);

                newItemDrop.transform.Rotate(Vector3.forward, Random.Range(-80, 80));

                newItemDrop.GetComponent<ItemPickup>().item = item;
                newItemDrop.GetComponent<ItemPickup>().ammount = requiredLootItemAmmounts[count];

                count++;
            }
        }
        else
        {
            chestSpriteRenderer.sprite = closedSprite;
        }
    }
}
