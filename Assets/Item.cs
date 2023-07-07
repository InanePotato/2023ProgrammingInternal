using System.Collections;
using System.Collections.Generic;
using System.IO.Enumeration;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName ="New Item", menuName ="Item Details")]
public class Item : ScriptableObject
{
    public Sprite sprite;
    public string itemName;
    public int inventoryId;
    public string description;
    public ItemType type;
    public Effects primaryEffect;
    public Effects secondaryEffect;
    public int ammount;
    
    public enum ItemType
    {
        currency, key, armor, weapon, consumable, misc
    }
    public enum Effects
    {
        none, burn, invisibility, resistance, strength, regeneration
    }
}
