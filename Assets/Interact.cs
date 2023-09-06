using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Interact : MonoBehaviour
{
    // Declare class scope variable for referencing the game manager script
    [Header("Game Manager")]
    private GameManagerScript gameManagerScript;

    // Declare class scope variable for keybind
    [Header("Keybinds")]
    private KeyCode interactKeybind;

    // Declare class scope variable for player info
    [Header("Player Information")]
    private GameObject playerObject;

    // Declare class scope variables for interaction info
    [Header("Interaction Information")]
    public Item requiredItem;
    private bool canInteract;
    private bool Interacted;
    public InteractionType interactionType;
    private GameObject popupMessage = null;
    public float interactMessageCooldown = 3;
    private float interactMessageCooldownCountdown = 0;

    public enum InteractionType { Door, Chest, Item, NPC };

    // Start is called before the first frame update
    void Start()
    {
        // Get and set reference to the game manager script
        gameManagerScript = GameManagerScript.Instance;

        // Get and set reference to the player object
        playerObject = GameObject.FindGameObjectsWithTag("Player").First();

        // Get and set interaction keybind from game manager
        interactKeybind = gameManagerScript.InteractKeybind;

        // Default can interact and interacted to false
        canInteract = false;
        Interacted = false;
    }

    // Update is called once per frame
    void Update()
    {
        // IF interact keybind pressed, can and haven’t interacted, and game is still running
        if (Input.GetKeyDown(interactKeybind) && canInteract && !Interacted && !gameManagerScript.gamePaused && !gameManagerScript.gameOver)
        {
            // IF interaction  type is door
            if (interactionType == InteractionType.Door)
            {
                // Call on door interact method
                DoorInteraction();
            }
            else if (interactionType == InteractionType.Chest)
            {
                // ELSE IF interaction type is chest
                // Call on chest interact method
                ChestInteraction();
            }
            else if (interactionType == InteractionType.NPC)
            {
                // ELSE IF interaction type is NPC
                // Call on npc interact method
                NPCInteraction();
            }
        }

        // IF interact message cooldown hasn’t finished
        if (interactMessageCooldownCountdown > 0)
        {
            // Subtract from cooldown timer
            interactMessageCooldownCountdown -= Time.deltaTime;
        }
    }

    /// <summary>
    /// handles spesifics to interacting with a door
    /// </summary>
    private void DoorInteraction()
    {
        // IF no required item
        if (requiredItem == null)
        {
            // Set to interacted
            Interacted = true;
            // Call on doors open door method
            gameObject.GetComponent<DoorScript>().openDoor(true);

            // Call on DestroyHintPopup method
            DestroyHintPopup();
        }
        else
        {
            // IF player has the required item
            if (InventoryManager.Instance.GetItemDetails(requiredItem) != null)
            {
                // Take one of the required item away
                InventoryManager.Instance.SubtractItemAmmount(requiredItem, 1);

                // Set to interacted
                Interacted = true;
                // Call on doors open door method
                gameObject.GetComponent<DoorScript>().openDoor(true);

                // Call on DestroyHintPopup method
                DestroyHintPopup();
            }
            else
            {
                // IF interact message cooldown has finished
                if (interactMessageCooldownCountdown <= 0)
                {
                    // Call on DisplayMessage in game managers script to spawn a failed message
                    gameManagerScript.DisplayMessage(requiredItem.itemName + " required", gameObject, Color.red);
                    // Reset interact message cooldown
                    interactMessageCooldownCountdown = interactMessageCooldown;
                }
            }
        }
    }

    /// <summary>
    /// Handles chest interaction spesifics
    /// </summary>
    private void ChestInteraction()
    {
        // IF chest isn’t open
        if (!gameObject.GetComponent<ChestScript>().chestOpen)
        {
            // IF no required item
            if (requiredItem == null)
            {
                // Set interacted to true
                Interacted = true;
                // Call on chests open chest script
                gameObject.GetComponent<ChestScript>().openChest(true);

                // Call on DestroyHintPopup method
                DestroyHintPopup();
            }
            else
            {
                // IF player has required item
                if (InventoryManager.Instance.GetItemDetails(requiredItem) != null)
                {
                    // Take one of the required item away
                    InventoryManager.Instance.SubtractItemAmmount(requiredItem, 1);

                    // Set interacted to true
                    Interacted = true;
                    // Call on chests open chest script
                    gameObject.GetComponent<ChestScript>().openChest(true);

                    // Call on DestroyHintPopup method
                    DestroyHintPopup();
                }
                else
                {
                    // IF interact message cooldown has finished
                    if (interactMessageCooldownCountdown <= 0)
                    {
                        // Call on DisplayMessage in game managers script to spawn a failed message
                        gameManagerScript.DisplayMessage(requiredItem.itemName + " required", gameObject, Color.red);
                        // Reset interact message cooldown
                        interactMessageCooldownCountdown = interactMessageCooldown;
                    }
                }
            }
            
        }
    }

    /// <summary>
    /// Handles NPc interaction specifics
    /// </summary>
    private void NPCInteraction()
    {
        // Call on DestroyHintPopup method
        DestroyHintPopup();

        // Call on StartInteraction method in the objects NPC script
        NPCScript objectScript = gameObject.GetComponent<NPCScript>();
        objectScript.StartInteraction();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // IF colliding with the player
        if (collision.gameObject == playerObject)
        {
            // Set can interact to true
            canInteract = true;

            // IF hasn’t interacted
            if (!Interacted)
            {
                // IF interaction type is an NPC
                if (interactionType ==  InteractionType.NPC)
                {
                    // Spawn and set a popup message prompting to ‘talk’
                    popupMessage = gameManagerScript.DisplayInteractPopup(interactKeybind.ToString(), "Talk", gameObject);
                }
                else
                {
                    // Spawn and set a popup message prompting to interact
                    popupMessage = gameManagerScript.DisplayInteractPopup(interactKeybind.ToString(), "Open " + interactionType.ToString(), gameObject);
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // IF player has stopped colliding
        if (collision.gameObject == playerObject)
        {
            // Set can interact to false
            canInteract = false;

            // Destroy the popup message
            DestroyHintPopup();

            // IF interaction type is an NPC
            if (interactionType == InteractionType.NPC)
            {
                // Call on the objects NPC script’s TerminateInteraction method
                gameObject.GetComponent<NPCScript>().TerminateInteraction();
            }
        }
    }

    /// <summary>
    /// Handles destruction/despawning of hint/interact popup
    /// </summary>
    private void DestroyHintPopup()
    {
        // IF popup exists
        if (popupMessage != null)
        {
            // Call on the objects destroy popup method
            popupMessage.GetComponent<PopupHintMessageDisplayScript>().DestroyPopup();
            // Set popup to null
            popupMessage = null;
        }
    }
}
