using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Inventory;

public class ItemScript : MonoBehaviour
{
    [Header("Game Manager")]
    private GameObject gameManager;
    private GameManagerScript gameManagerScript;

    [Header("Player Information")]
    private GameObject playerObject;
    private Inventory playerInventory;

    [Header("Item Information")]
    public ItemNames itemName;
    public int ammount;
    public int pickupCooldwon = 1;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindGameObjectsWithTag("GameController").First();
        gameManagerScript = gameManager.GetComponent<GameManagerScript>();

        playerObject = GameObject.FindGameObjectsWithTag("Player").First();
        playerInventory = playerObject.GetComponent<Inventory>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == playerObject)
        {
            playerInventory.updateInventoryItemAmmount(itemName, ammount);
            Destroy(gameObject, pickupCooldwon);
        }
    }
}
