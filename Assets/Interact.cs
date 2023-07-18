using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Interact : MonoBehaviour
{
    [Header("Game Manager")]
    private GameManagerScript gameManagerScript;

    [Header("Keybinds")]
    private KeyCode interactKeybind;

    [Header("Player Information")]
    private GameObject playerObject;

    [Header("Interaction Information")]
    public Item requiredItem;
    private bool canInteract;
    private bool Interacted;
    public InteractionType interactionType;

    public enum InteractionType { Door, Chest, Item };

    // Start is called before the first frame update
    void Start()
    {
        // gets the game manager script
        gameManagerScript = GameManagerScript.Instance;

        playerObject = GameObject.FindGameObjectsWithTag("Player").First();

        interactKeybind = gameManagerScript.InteractKeybind;

        canInteract = false;
        Interacted = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(interactKeybind) && canInteract && !Interacted && !gameManagerScript.gamePaused && !gameManagerScript.gameOver)
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
        if (requiredItem == null)
        {
            Interacted = true;
            gameObject.GetComponent<DoorScript>().openDoor(true);
        }
        else
        {
            if (InventoryManager.Instance.GetItemDetails(requiredItem) != null)
            {
                InventoryManager.Instance.SubtractItemAmmount(requiredItem, 1);

                Interacted = true;
                gameObject.GetComponent<DoorScript>().openDoor(true);
            }
            else
            {
                gameManagerScript.DisplayMessage(requiredItem.itemName + " required", gameObject, Color.red);
                Debug.Log("Can't open door");
            }
        }
    }

    private void ChestInteraction()
    {
        if (!gameObject.GetComponent<ChestScript>().chestOpen)
        {
            if (requiredItem == null)
            {
                Interacted = true;
                gameObject.GetComponent<ChestScript>().openChest(true);
            }
            else
            {
                if (InventoryManager.Instance.GetItemDetails(requiredItem) != null)
                {
                    InventoryManager.Instance.SubtractItemAmmount(requiredItem, 1);

                    Interacted = true;
                    gameObject.GetComponent<ChestScript>().openChest(true);
                }
                else
                {
                    gameManagerScript.DisplayMessage(requiredItem.itemName + " required", gameObject, Color.red);
                    Debug.Log("Can't open chest");
                }
            }
            
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
