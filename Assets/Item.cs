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
    //public string description;
    public ItemType type;
    public Effects primaryEffect;
    public Effects secondaryEffect;
    public int ammount;
    public bool equiped;
    public float damage;
    public float protection;
    public Ability ability;
    public float abilityValue;
    
    public enum ItemType
    {
        currency, key, hat, weapon, consumable, misc, spell, chestplate, boots, relic
    }
    public enum Effects
    {
        none, burn, invisibility, resistance, strength, regeneration
    }
    public enum Ability
    {
        none, extraHealth, extraSpellDamage, extraWeaponDamage, extraSpeed
    }
}
