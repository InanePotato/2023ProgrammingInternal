using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [Header("Game Manager")]
    private GameManagerScript gameManagerScript;

    [Header("Player Information")]
    private GameObject playerObject;
    private InventoryManager playerInventory;

    [Header("Item Details")]
    public Item item;
    public int ammount;
    private float pickupCooldwon = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        // sets the sprite for this object
        gameObject.GetComponent<SpriteRenderer>().sprite = item.sprite;

        // gets the game manager script
        gameManagerScript = GameManagerScript.Instance;

        // gets the player object and inventory script
        playerInventory = InventoryManager.Instance;
        playerObject = gameManagerScript.player;
    }

    /// <summary>
    /// Whenever another object collides with this object...
    /// checks if it's the player and if so then adds the item to the inventory and destroys this object.
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == playerObject)
        {
            playerInventory.AddItemAmmount(item, ammount);
            Destroy(gameObject, pickupCooldwon);
        }
    }
}
