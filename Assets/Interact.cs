using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Inventory;

public class Interact : MonoBehaviour
{
    [Header("Game Manager")]
    private GameObject gameManager;
    private GameManagerScript gameManagerScript;

    [Header("Keybinds")]
    private KeyCode interactKeybind;

    [Header("Player Information")]
    private GameObject playerObject;
    private Inventory playerInventory;

    [Header("Interaction Information")]
    public Keys keyNeeded = Keys.None;
    private bool canInteract;
    private bool Interacted;
    public InteractionType interactionType;

    public enum InteractionType { Door, Chest, Item };
    public enum Keys { None, Wodden_Key, Silver_Key, Gold_Key, Diamond_Key, Special_Key };

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindGameObjectsWithTag("GameController").First();
        gameManagerScript = gameManager.GetComponent<GameManagerScript>();

        playerObject = GameObject.FindGameObjectsWithTag("Player").First();
        playerInventory = playerObject.GetComponent<Inventory>();

        interactKeybind = gameManagerScript.InteractKeybind;

        canInteract = false;
        Interacted = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(interactKeybind) && canInteract && !Interacted && !gameManagerScript.gamePaused)
        {
            if (interactionType == InteractionType.Door)
            {
                DoorInteraction();
            }
            else if (interactionType == InteractionType.Chest)
            {
                ChestInteraction();
            }
        }
    }

    private void DoorInteraction()
    {
        if (keyNeeded == Keys.None)
        {
            Interacted = true;
            gameObject.GetComponent<DoorScript>().openDoor(true);
        }
        else
        {
            if (playerInventory.checkHasItem((ItemNames)Enum.Parse(typeof(ItemNames), keyNeeded.ToString())))
            {
                playerInventory.updateInventoryItemAmmount((ItemNames)Enum.Parse(typeof(ItemNames), keyNeeded.ToString()), -1);

                Interacted = true;
                gameObject.GetComponent<DoorScript>().openDoor(true);
            }
            else
            {
                Debug.Log("Can't open door, not enough keys of type: " + keyNeeded);
            }
        }
    }

    private void ChestInteraction()
    {
        if (playerInventory.checkHasItem((ItemNames)Enum.Parse(typeof(ItemNames), keyNeeded.ToString())))
        {
            playerInventory.updateInventoryItemAmmount((ItemNames)Enum.Parse(typeof(ItemNames), keyNeeded.ToString()), -1);

            Interacted = true;
            gameObject.GetComponent<ChestScript>().openChest(true);
        }
        else
        {
            Debug.Log("Can't open chest, not enough keys of type: " + keyNeeded);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == playerObject)
        {
            canInteract = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        canInteract = false;
    }
}
