using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;
using static Inventory;
using static UnityEditor.Progress;

public class ChestScript : MonoBehaviour
{
    [Header("Game Manager")]
    private GameObject gameManager;
    private GameManagerScript gameManagerScript;

    [Header("Chest Information")]
    public ChestType chestType;
    private bool chestOpen = false;
    private SpriteRenderer chestSpriteRenderer;
    private Transform chestTransform;
    public float lootSpawnOffsetY;
    public float lootSpawnOffsetX;
    const float LOOT_SPAWN_DISTANCE_BELOW = 0.2f;
    public Sprite openSprite;
    public Sprite closedSprite;

    [Header("Required Loot")]
    public List<GameObject> requiredLootItemObjects = new List<GameObject>();
    public List<int> requiredLootItemAmmounts = new List<int>();

    [Header("Random Loot")]
    public int numberOfRandoms;
    private List<GameObject> randomLootTableObjects = new List<GameObject>();
    private List<int> randomLootTableMaxAmmounts = new List<int>();

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
        gameManager = GameObject.FindGameObjectsWithTag("GameController").First();
        gameManagerScript = gameManager.GetComponent<GameManagerScript>();

        chestSpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        chestTransform = gameObject.GetComponent<Transform>();

        if (chestType == ChestType.SilverChest)
        {
            randomLootTableObjects = gameManagerScript.randomSilverLootTableObjects;
            randomLootTableMaxAmmounts = gameManagerScript.randomSilverLootTableMaxAmmounts;
        }
        else
        {
            randomLootTableObjects = gameManagerScript.randomGoldLootTableObjects;
            randomLootTableMaxAmmounts = gameManagerScript.randomGoldLootTableMaxAmmounts;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void openChest(bool isOpen)
    {
        chestOpen = isOpen;

        if (chestOpen)
        {
            chestSpriteRenderer.sprite = openSprite;

            // release/spawn in loot
            int count = 0;
            foreach (GameObject item in requiredLootItemObjects)
            {
                GameObject newItemDrop = Instantiate(item);

                float X = chestTransform.position.x + Random.Range(-lootSpawnOffsetX, lootSpawnOffsetX);
                float Y = chestTransform.position.y + Random.Range(-lootSpawnOffsetY, lootSpawnOffsetY) - LOOT_SPAWN_DISTANCE_BELOW;
                newItemDrop.transform.position = new Vector2(X, Y);

                newItemDrop.transform.Rotate(Vector3.forward, Random.Range(-80, 80));

                newItemDrop.GetComponent<ItemScript>().ammount = requiredLootItemAmmounts[count];

                count++;
            }

            for (int i = 0; i < numberOfRandoms; i++)
            {
                int randomId = Random.Range(0, randomLootTableObjects.Count());

                GameObject newItemDrop = Instantiate(randomLootTableObjects[randomId]);

                float X = chestTransform.position.x + Random.Range(-lootSpawnOffsetX, lootSpawnOffsetX);
                float Y = chestTransform.position.y + Random.Range(-lootSpawnOffsetY, lootSpawnOffsetY) - LOOT_SPAWN_DISTANCE_BELOW;
                newItemDrop.transform.position = new Vector2(X, Y);

                newItemDrop.GetComponent<ItemScript>().ammount = Random.Range(1, randomLootTableMaxAmmounts[randomId]);
            }
        }
        else
        {
            chestSpriteRenderer.sprite = closedSprite;
        }
    }
}
