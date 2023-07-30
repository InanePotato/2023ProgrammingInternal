using Microsoft.Unity.VisualStudio.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEditor.Progress;

public class PlayerStats : MonoBehaviour
{
    GameManagerScript gameManagerScript;
    InventoryManager inventoryManager;
    List<Item> inventoryItemsList = new List<Item>();

    [Header("UI")]
    public GameObject EquippedSlotsPanel;
    private GameObject[] imgSlot = new GameObject[6];

    [Header("Equipped Items")]
    public Item[] equippedSlot = new Item[6];

    [Header("Health")]
    public float maxHealth;
    private float health;
    public GameObject pnlHealthBar;
    private GameObject pnlHealthBarDisplay;
    private float initialHealthBarWidth;
    private float healthBarWidthIncrement;

    [Header("Protection")]
    public float totalProtection;
    public float[] slotProtection = new float[6];

    [Header("Attack")]
    public float defaultDamage;
    public float weaponDamage;
    public float weaponDamageMultiplier;
    public float spellDamage;
    public float spellDamageMultiplier;

    // Start is called before the first frame update
    void Start()
    {
        gameManagerScript = GameManagerScript.Instance;
        inventoryManager = gameObject.GetComponent<InventoryManager>();
        inventoryItemsList = inventoryManager.Items;

        health = maxHealth;

        totalProtection = 0;

        pnlHealthBarDisplay = pnlHealthBar.transform.GetChild(0).gameObject;
        initialHealthBarWidth = pnlHealthBarDisplay.GetComponent<RectTransform>().sizeDelta.x;
        healthBarWidthIncrement = initialHealthBarWidth / maxHealth;

        weaponDamage = defaultDamage;

        imgSlot[0] = EquippedSlotsPanel.transform.Find("HatSlot").gameObject;
        imgSlot[1] = EquippedSlotsPanel.transform.Find("ChestplateSlot").gameObject;
        imgSlot[2] = EquippedSlotsPanel.transform.Find("BootsSlot").gameObject;
        imgSlot[3] = EquippedSlotsPanel.transform.Find("WeaponSlot").gameObject;
        imgSlot[4] = EquippedSlotsPanel.transform.Find("SpellSlot").gameObject;
        imgSlot[5] = EquippedSlotsPanel.transform.Find("RelicSlot").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddHealth(float ammount)
    {
        health += ammount;

        if (health > maxHealth)
        {
            health = maxHealth;
        }

        pnlHealthBarDisplay.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, health * healthBarWidthIncrement);
    }

    public void SubtractHealth(float ammount)
    {
        health -= ammount;

        if (health <= 0)
        {
            // Player dead
            gameManagerScript.EndGame("Player died");
            return;
        }

        float currentWidth = pnlHealthBarDisplay.GetComponent<RectTransform>().rect.width;
        pnlHealthBarDisplay.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, health * healthBarWidthIncrement);
    }

    public void EquipItem(Item item)
    {
        item.equiped = true;
        Item.ItemType type = item.type;
        
        int arrayIndex = 0;
        if (type == Item.ItemType.chestplate) { arrayIndex = 1; }
        else if (type == Item.ItemType.boots) { arrayIndex = 2; }
        else if (type == Item.ItemType.weapon) { arrayIndex = 3; }
        else if (type == Item.ItemType.spell) { arrayIndex = 3; }
        else if (type == Item.ItemType.relic) { arrayIndex = 4; }

        // unequip current item of type
        UnEquipItem(type);

        // equip new item of type
        equippedSlot[arrayIndex] = item;

        // add any protection
        slotProtection[arrayIndex] = item.protection;
        UpdateProtection();

        // handle any abilities given
        if (item.ability != Item.Ability.none)
        {
            HandleAbilityChange(item, true);
        }

        // change sprite
        var iconImage = imgSlot[arrayIndex].GetComponent<UnityEngine.UI.Image>();
        iconImage.sprite = item.sprite;
        iconImage.color = new Color(255, 255, 255, 255);
    }

    public void UnEquipItem(Item.ItemType type)
    {
        int arrayIndex = 0;
        if (type == Item.ItemType.chestplate) { arrayIndex = 1; }
        else if (type == Item.ItemType.boots) { arrayIndex = 2; }
        else if (type == Item.ItemType.weapon) { arrayIndex = 3; }
        else if (type == Item.ItemType.spell) { arrayIndex = 3; }
        else if (type == Item.ItemType.relic) { arrayIndex = 4; }

        // it there is not sonthing to un-equip, don't continue
        if (equippedSlot[arrayIndex] == null)
        {
            return;
        }

        equippedSlot[arrayIndex].equiped = false;

        // add any protection
        slotProtection[arrayIndex] = 0;
        UpdateProtection();

        // handle any abilities given
        if (equippedSlot[arrayIndex].ability != Item.Ability.none)
        {
            HandleAbilityChange(equippedSlot[arrayIndex], false);
        }

        // change sprite
        var iconImage = imgSlot[arrayIndex].GetComponent<UnityEngine.UI.Image>();
        iconImage.sprite = null;
        iconImage.color = new Color32(144, 144, 144, 60);
        Debug.Log("Reverting Colours");

        // un-equip new item of type
        equippedSlot[arrayIndex].equiped = false;
        equippedSlot[arrayIndex] = null;
    }

    public void UpdateProtection()
    {
        totalProtection = 0;
        for (int i = 0; i < slotProtection.Length; i++)
        {
            totalProtection += slotProtection[i];
        }
    }

    public void HandleAbilityChange(Item item, bool addAbility)
    {
        float multiplier = 1;
        if (addAbility == false)
        {
            multiplier = -1;
        }

        // handle removal/addition of  abilities
        if (item.ability == Item.Ability.extraHealth)
        {
            maxHealth += item.abilityValue * multiplier;
            healthBarWidthIncrement = initialHealthBarWidth / maxHealth;
        }
        else if (item.ability == Item.Ability.extraWeaponDamage)
        {
            weaponDamageMultiplier = item.abilityValue * multiplier;
        }
        else if (item.ability == Item.Ability.extraSpellDamage)
        {
            spellDamageMultiplier = item.abilityValue * multiplier;
        }
    }

    public void UseItem(Item item)
    {
        item.ammount--;
    }
}
