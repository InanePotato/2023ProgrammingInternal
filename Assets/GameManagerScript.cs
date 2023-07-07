using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    public static GameManagerScript Instance;

    [Header("Keybinds")]
    public KeyCode ToggleMenuKeybind = KeyCode.Escape;
    public KeyCode ToggleInventoryKeybind = KeyCode.I;
    public KeyCode InteractKeybind = KeyCode.E;

    [Header("Game Control")]
    public bool gamePaused = false;

    [Header("Player")]
    public GameObject player;
    public GameObject playerSprite;
    public PlayerSprites selectedPlayerSprite;

    [Header("Items")]
    public GameObject droppedItemPrefab;

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

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // gets the player object
        player = GameObject.FindGameObjectsWithTag("Player").First();

        // sets instanciate to chosen player sprite
        if (selectedPlayerSprite == PlayerSprites.Knight_Original) { playerSprite = Instantiate(spritePrefab_KnightOriginal, Vector2.zero, Quaternion.identity); }
        else if (selectedPlayerSprite == PlayerSprites.Knight_Pink) { playerSprite = Instantiate(spritePrefab_KnightPink, Vector2.zero, Quaternion.identity); }
        else { playerSprite = Instantiate(spritePrefab_KnightRed, Vector2.zero, Quaternion.identity); }

        // adds sprite as a child of the player
        playerSprite.transform.parent = player.transform;

        // sets the player with the correct sprite
        player.GetComponent<PlayerMovement>().setPlayerSprite();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
