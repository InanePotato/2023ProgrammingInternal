using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    [Header("Keybinds")]
    public KeyCode OpenMenuKeybind = KeyCode.Escape;
    public KeyCode OpenInventoryKeybind = KeyCode.I;
    public KeyCode InteractKeybind = KeyCode.E;

    [Header("Player")]
    public GameObject player;
    public GameObject playerSprite;
    public PlayerSprites selectedPlayerSprite;

    [Header("Level Spawns")]
    public List<GameObject> objectsToSpawn;

    [Header("Loot Tables - Silver")]
    public List<GameObject> randomSilverLootTableObjects = new List<GameObject>();
    public List<int> randomSilverLootTableMaxAmmounts = new List<int>();

    [Header("Loot Tables - Gold")]
    public List<GameObject> randomGoldLootTableObjects = new List<GameObject>();
    public List<int> randomGoldLootTableMaxAmmounts = new List<int>();

    public enum PlayerSprites
    {
        Knight_Red,
        Knight_Pink,
        Knight_Original
    }

    [Header("Player Sprites")]
    public GameObject spritePrefab_KnightRed;
    public GameObject spritePrefab_KnightPink;
    public GameObject spritePrefab_KnightOriginal;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectsWithTag("Player").First();

        // sets instanciate to chosen player sprite
        if (selectedPlayerSprite == PlayerSprites.Knight_Original) { playerSprite = Instantiate(spritePrefab_KnightOriginal, Vector2.zero, Quaternion.identity); }
        else if (selectedPlayerSprite == PlayerSprites.Knight_Pink) { playerSprite = Instantiate(spritePrefab_KnightPink, Vector2.zero, Quaternion.identity); }
        else { playerSprite = Instantiate(spritePrefab_KnightRed, Vector2.zero, Quaternion.identity); }

        // adds sprite as a child of the player
        playerSprite.transform.parent = player.transform;

        player.GetComponent<PlayerMovement>().setPlayerSprite();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
