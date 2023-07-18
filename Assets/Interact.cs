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
    private GameObject popupMessage = null;
    public float interactMessageCooldown = 3;
    private float interactMessageCooldownCountdown = 0;

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

        if (interactMessageCooldownCountdown > 0)
        {
            interactMessageCooldownCountdown -= Time.deltaTime;
        }
    }

    private void DoorInteraction()
    {
        if (requiredItem == null)
        {
            Interacted = true;
            gameObject.GetComponent<DoorScript>().openDoor(true);

            DestroyHintPopup();
        }
        else
        {
            if (InventoryManager.Instance.GetItemDetails(requiredItem) != null)
            {
                InventoryManager.Instance.SubtractItemAmmount(requiredItem, 1);

                Interacted = true;
                gameObject.GetComponent<DoorScript>().openDoor(true);

                DestroyHintPopup();
            }
            else
            {
                if (interactMessageCooldownCountdown <= 0)
                {
                    gameManagerScript.DisplayMessage(requiredItem.itemName + " required", gameObject, Color.red);
                    interactMessageCooldownCountdown = interactMessageCooldown;
                }
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

                DestroyHintPopup();
            }
            else
            {
                if (InventoryManager.Instance.GetItemDetails(requiredItem) != null)
                {
                    InventoryManager.Instance.SubtractItemAmmount(requiredItem, 1);

                    Interacted = true;
                    gameObject.GetComponent<ChestScript>().openChest(true);

                    DestroyHintPopup();
                }
                else
                {
                    if (interactMessageCooldownCountdown <= 0)
                    {
                        gameManagerScript.DisplayMessage(requiredItem.itemName + " required", gameObject, Color.red);
                        interactMessageCooldownCountdown = interactMessageCooldown;
                    }
                }
            }
            
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == playerObject)
        {
            canInteract = true;

            if (!Interacted)
            {
                popupMessage = gameManagerScript.DisplayInteractPopup(interactKeybind.ToString(), "Open " + interactionType.ToString(), gameObject);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        canInteract = false;

        DestroyHintPopup();
    }

    private void DestroyHintPopup()
    {
        if (popupMessage != null)
        {
            popupMessage.GetComponent<PopupHintMessageDisplayScript>().DestroyPopup();
            popupMessage = null;
        }
    }
}
