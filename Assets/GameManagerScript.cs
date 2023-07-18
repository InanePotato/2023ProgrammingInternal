using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerScript : MonoBehaviour
{
    public static GameManagerScript Instance;
    public GameObject cameraObject;
    private Camera camera;

    [Header("Keybinds")]
    public KeyCode ToggleMenuKeybind = KeyCode.Escape;
    public KeyCode ToggleInventoryKeybind = KeyCode.I;
    public KeyCode InteractKeybind = KeyCode.E;

    [Header("Game Control")]
    public bool gamePaused = false;
    public bool gameOver = false;

    [Header("Player")]
    public GameObject player;
    public GameObject playerSprite;
    public PlayerSprites selectedPlayerSprite;

    [Header("Items")]
    public GameObject droppedItemPrefab;

    [Header("UI")]
    public GameObject mainCanvis;
    private GameObject pnlPauseMenu;
    private GameObject pnlTopGUI;
    public GameObject txtMessageDisplayPrefab;
    public GameObject pnlPopupDisplayPrefab;
    public float txtMessageDisplayTimeout = 2;

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

        pnlPauseMenu = mainCanvis.transform.Find("PauseMenu").gameObject;
        pnlTopGUI = mainCanvis.transform.Find("TopGUI").gameObject;

        camera = cameraObject.GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void EndGame(string reason)
    {
        // get rid of any pause that is on the game
        TogglePauseScreen(false);

        // set game over to true
        gameOver = true;
    }

    public void TogglePauseScreen(bool showScreen)
    {
        if (!gameOver)
        {
            // Toggle game paused bool
            gamePaused = showScreen;

            // Hide all current popups
            player.GetComponent<InventoryManager>().toggleInventory(false);

            // Toggle visibility of pause button
            pnlTopGUI.transform.Find("btnPause").gameObject.SetActive(!showScreen);

            // Toggle visibility of pause UI panel
            pnlPauseMenu.SetActive(showScreen);
        }
    }

    public void DisplayMessage(string message, GameObject spawnOnObject, Color colour)
    {
        GameObject newMessage = Instantiate(txtMessageDisplayPrefab, mainCanvis.transform);

        newMessage.GetComponent<Text>().text = message;
        newMessage.GetComponent<Text>().color = colour;
        newMessage.GetComponent<RectTransform>().position = camera.WorldToScreenPoint(spawnOnObject.transform.position);

        Destroy(newMessage, txtMessageDisplayTimeout);
    }

    public GameObject DisplayInteractPopup(string key, string message, GameObject spawnOnObject)
    {
        GameObject newPopup = Instantiate(pnlPopupDisplayPrefab, mainCanvis.transform);

        newPopup.transform.Find("txtKey").GetComponent<Text>().text = key;
        newPopup.transform.Find("txtDescription").GetComponent<Text>().text = message;
        newPopup.GetComponent<RectTransform>().position = camera.WorldToScreenPoint(spawnOnObject.transform.position);

        return newPopup;
    }
}
