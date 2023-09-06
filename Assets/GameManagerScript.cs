using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerScript : MonoBehaviour
{
    // Declare class scope static variable for referencing this instance of the script
    public static GameManagerScript Instance;
    public GameObject cameraObject;
    public Camera camera;

    // Declare class scope variables for keybinds
    [Header("Keybinds")]
    public KeyCode ToggleMenuKeybind = KeyCode.Escape;
    public KeyCode ToggleInventoryKeybind = KeyCode.I;
    public KeyCode InteractKeybind = KeyCode.E;

    // Declare class scope variables for game control info
    [Header("Game Control")]
    public bool gamePaused = false;
    public bool gameOver = false;

    // Declare class scope variables for Player stuff
    [Header("Player")]
    public GameObject player;
    public GameObject playerSprite;
    public PlayerSprites selectedPlayerSprite;

    [Header("Items")]
    public GameObject droppedItemPrefab;

    // Declare class scope variables for UI
    [Header("UI")]
    public GameObject mainCanvis;
    private GameObject pnlPauseMenu;
    private GameObject pnlTopGUI;
    public GameObject txtMessageDisplayPrefab;
    public GameObject pnlPopupDisplayPrefab;
    public float txtMessageDisplayTimeout = 2;
    public GameObject helpMenu;
    private bool helpMenuVisible = false;

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
        // 	Get and set reference to camera object
        Instance = this;
        // Set static reference to this instance of the script
        camera = cameraObject.GetComponent<Camera>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Get and set reference to player object
        player = GameObject.FindGameObjectsWithTag("Player").First();

        // Set player sprite
        if (selectedPlayerSprite == PlayerSprites.Knight_Original) { playerSprite = Instantiate(spritePrefab_KnightOriginal); }
        else if (selectedPlayerSprite == PlayerSprites.Knight_Pink) { playerSprite = Instantiate(spritePrefab_KnightPink); }
        else { playerSprite = Instantiate(spritePrefab_KnightRed); }
        playerSprite.transform.parent = player.transform;
        player.GetComponent<PlayerMovement>().setPlayerSprite();

        // Get and set references to pause and topGUI canvas panels
        pnlPauseMenu = mainCanvis.transform.Find("PauseMenu").gameObject;
        pnlTopGUI = mainCanvis.transform.Find("TopGUI").gameObject;
    }

    /// <summary>
    /// Handles end game/game over instances
    /// </summary>
    /// <param name="reason"></param>
    public void EndGame(string reason)
    {
        // Make sure pause screen is hidden
        TogglePauseScreen(false);

        // Set the game to be over
        gameOver = true;

        // Show game over screen


        // Display Game over reason

    }

    /// <summary>
    /// Handles toggling of pause menu
    /// </summary>
    /// <param name="showScreen"></param>
    public void TogglePauseScreen(bool showScreen)
    {
        // IF the game isn’t over
        if (!gameOver)
        {
            // Set paused game bool using given bool
            gamePaused = showScreen;

            // Make sure the players inventory is hidden
            player.GetComponent<InventoryManager>().toggleInventory(false);

            // Toggle visibility of pause button
            pnlTopGUI.transform.Find("btnPause").gameObject.SetActive(!showScreen);

            // Toggle visibility of pause UI panel
            pnlPauseMenu.SetActive(showScreen);
        }
    }

    /// <summary>
    /// handles spawning of message displays
    /// </summary>
    /// <param name="message"></param>
    /// <param name="spawnOnObject"></param>
    /// <param name="colour"></param>
    public void DisplayMessage(string message, GameObject spawnOnObject, Color colour)
    {
        // Spawn in a new message game object as a child of the canvas
        GameObject newMessage = Instantiate(txtMessageDisplayPrefab, mainCanvis.transform.Find("LevelSpawns"));

        // Set message and text colour appropriately
        newMessage.GetComponent<Text>().text = message;
        newMessage.GetComponent<Text>().color = colour;
        // Set location based on given object location
        newMessage.GetComponent<RectTransform>().position = camera.WorldToScreenPoint(spawnOnObject.transform.position);

        // Destroy message after timeout period
        Destroy(newMessage, txtMessageDisplayTimeout);
    }

    /// <summary>
    /// Handles spawning of interact popup
    /// </summary>
    /// <param name="key"></param>
    /// <param name="message"></param>
    /// <param name="spawnOnObject"></param>
    /// <returns></returns>
    public GameObject DisplayInteractPopup(string key, string message, GameObject spawnOnObject)
    {
        // Spawn in a new interact popup game object as a child of the canvas
        GameObject newPopup = Instantiate(pnlPopupDisplayPrefab, mainCanvis.transform.Find("LevelSpawns"));

        // Set key and message text appropriately
        newPopup.transform.Find("txtKey").GetComponent<Text>().text = key;
        newPopup.transform.Find("txtDescription").GetComponent<Text>().text = message;
        // Set location based on given object location
        newPopup.GetComponent<RectTransform>().position = camera.WorldToScreenPoint(spawnOnObject.transform.position);

        // Return the spawned in object
        return newPopup;
    }

    /// <summary>
    /// Handles toggling of help ment
    /// </summary>
    public void ToggleHelpMenu()
    {
        // Get animator component of the menu
        Animator animator = helpMenu.GetComponent<Animator>();

        // IF currently showing
        if (helpMenuVisible)
        {
            // Play hide animation
            animator.Play("helpMenuHide");
            // Set visible to false
            helpMenuVisible = false;
        }
        else
        {
            // Play show animation
            animator.Play("helpMenuShow");
            // Set visible to true
            helpMenuVisible = true;
        }
    }
}
