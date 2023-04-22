using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    [Header("Door Details")]
    public bool open = false;
    SpriteRenderer spriteRenderer;
    BoxCollider2D doorCollider;

    [Header("Door Type")]
    public DoorTypes doorType;
    public enum DoorTypes { Horizontal, Vertical };

    [Header("Sprites")]
    public Sprite openSprite;
    public Sprite closedSprite;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        doorCollider = gameObject.GetComponent<BoxCollider2D>();

        changeSprite();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void changeSprite()
    {
        if (open)
        {
            spriteRenderer.sprite = openSprite;
            doorCollider.enabled = false;
        }
        else
        {
            spriteRenderer.sprite = closedSprite;
            doorCollider.enabled = true;
        }
    }

    public void openDoor(bool isOpen)
    {
        open = isOpen;
        changeSprite();
    }
}
