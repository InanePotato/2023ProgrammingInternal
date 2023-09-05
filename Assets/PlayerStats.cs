using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PlayerStats : MonoBehaviour
{
    GameManagerScript gameManagerScript;
    InventoryManager inventoryManager;
    List<Item> inventoryItemsList = new List<Item>();

    [Header("UI")]
    public GameObject EquippedSlotsPanel;
    private GameObject[] imgSlot = new GameObject[6];
    public GameObject pnlStatsDisplay;
    private GameObject statsDisplayProtection;
    private GameObject statsDisplaySpeed;
    private GameObject statsDisplayWeaponDamage;
    private GameObject statsDisplaySpellDamage;

    [Header("Equipped Items")]
    public Item[] equippedSlot = new Item[6];

    [Header("Health")]
    public float maxHealth;
    private float health;
    public GameObject pnlHealthBar;
    private GameObject pnlHealthBarDisplay;
    private float initialHealthBarWidth;
    private float healthBarWidthIncrement;

    [Header("Score")]
    public int score;
    public GameObject txtScore;

    [Header("Protection")]
    public float totalProtection;
    public float[] slotProtection = new float[6];

    [Header("Speed")]
    public float speedMultiplier;

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

        statsDisplayProtection = pnlStatsDisplay.transform.Find("txtProtection").gameObject;
        statsDisplaySpeed = pnlStatsDisplay.transform.Find("txtSpeed").gameObject;
        statsDisplayWeaponDamage = pnlStatsDisplay.transform.Find("txtWeaponDamage").gameObject;
        statsDisplaySpellDamage = pnlStatsDisplay.transform.Find("txtSpellDamage").gameObject;

        UpdateStatsDisplay();
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
        float subtractAmmount = ammount - totalProtection;

        if (subtractAmmount < 0)
        {
            subtractAmmount = 0;
        }

        health -= subtractAmmount;

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
        else if (type == Item.ItemType.weapon || type == Item.ItemType.rangedWeapon) { arrayIndex = 3; }
        else if (type == Item.ItemType.spell) { arrayIndex = 4; }
        else if (type == Item.ItemType.relic) { arrayIndex = 5; }

        // unequip current item of type
        UnEquipItem(type);

        // set any damage the weapon or spell does
        if (type == Item.ItemType.weapon)
        {
            weaponDamage = item.damage;
        }
        else if (type == Item.ItemType.spell)
        {
            spellDamage = item.damage;
        }

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

        UpdateStatsDisplay();
    }

    public void UnEquipItem(Item.ItemType type)
    {
        int arrayIndex = 0;
        if (type == Item.ItemType.chestplate) { arrayIndex = 1; }
        else if (type == Item.ItemType.boots) { arrayIndex = 2; }
        else if (type == Item.ItemType.weapon || type == Item.ItemType.rangedWeapon) { arrayIndex = 3; weaponDamage = defaultDamage; }
        else if (type == Item.ItemType.spell) { arrayIndex = 4; spellDamage = 0; }
        else if (type == Item.ItemType.relic) { arrayIndex = 5; }

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

        // un-equip new item of type
        equippedSlot[arrayIndex].equiped = false;
        equippedSlot[arrayIndex] = null;

        UpdateStatsDisplay();
    }

    public void UpdateProtection()
    {
        totalProtection = 0;
        for (int i = 0; i < slotProtection.Length; i++)
        {
            totalProtection += slotProtection[i];
        }

        statsDisplayProtection.GetComponent<Text>().text = totalProtection.ToString();
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
            weaponDamageMultiplier += item.abilityValue * multiplier;
        }
        else if (item.ability == Item.Ability.extraSpellDamage)
        {
            spellDamageMultiplier += item.abilityValue * multiplier;
        }
        else if (item.ability == Item.Ability.extraSpeed)
        {
            speedMultiplier += item.abilityValue * multiplier;
        }
        else if (item.ability == Item.Ability.fasterAttack)
        {
            float preChange = gameObject.GetComponent<PlayerAttackScript>().attackCooldown;
            gameObject.GetComponent<PlayerAttackScript>().attackCooldown -= item.abilityValue * multiplier;

            UnityEngine.Debug.Log("Attack cooldown change: " + preChange + " => " + gameObject.GetComponent<PlayerAttackScript>().attackCooldown);
        }
    }

    public void UseItem(Item item)
    {
        // don't use if have none
        if (item.ammount <= 0)
        {
            return;
        }

        item.ammount--;

        if (item.ability == Item.Ability.healthBack)
        {
            AddHealth(item.abilityValue);
        }

        if (item.ammount <= 0)
        {
            inventoryManager.RemoveItem(item);
        }
    }

    private void UpdateStatsDisplay()
    {
        statsDisplayProtection.GetComponent<Text>().text = totalProtection.ToString();
        statsDisplaySpeed.GetComponent<Text>().text = (gameObject.GetComponent<PlayerMovement>().walkSpeed + speedMultiplier).ToString();
        statsDisplayWeaponDamage.GetComponent<Text>().text = (weaponDamage + weaponDamageMultiplier).ToString();
        statsDisplaySpellDamage.GetComponent<Text>().text = (spellDamage + spellDamageMultiplier).ToString();
    }

    public void ChangeScore(int ammount)
    {
        score += ammount;
        txtScore.GetComponent<Text>().text = "Score: " + score.ToString();
    }
}
