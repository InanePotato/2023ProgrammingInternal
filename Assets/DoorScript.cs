using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    // Declare Class scope variables for info on the door object
    [Header("Door Details")]
    public bool open = false;
    SpriteRenderer spriteRenderer;
    BoxCollider2D doorCollider;
    public DoorTypes doorType;

    // Declare Class scope variables for the different door sprites
    [Header("Sprites")]
    public Sprite openSprite;
    public Sprite closedSprite;

    // Declare public enum for door types
    public enum DoorTypes { Horizontal, Vertical };

    // Start is called before the first frame update
    void Start()
    {
        // Get and set a reference for the objects sprite renderer and collider components
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        doorCollider = gameObject.GetComponent<BoxCollider2D>();

        // Call on the ChangeSprite method to update sprites
        changeSprite();
    }

    /// <summary>
    /// Handles door sprite changes
    /// </summary>
    void changeSprite()
    {
        // IF door is open
        if (open)
        {
            // Switch to open sprite
            spriteRenderer.sprite = openSprite;
            // Disable objects collider
            doorCollider.enabled = false;
        }
        else
        {
            // door is closed
            // Switch to closed sprite
            spriteRenderer.sprite = closedSprite;
            // Enable objects collider
            doorCollider.enabled = true;
        }
    }

    /// <summary>
    /// Handles change of state of the door
    /// </summary>
    /// <param name="isOpen"></param>
    public void openDoor(bool isOpen)
    {
        // Set open variable to given value
        open = isOpen;
        // Call on ChangeSprite method
        changeSprite();
    }
}
