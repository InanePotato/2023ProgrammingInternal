using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    GameManagerScript gameManagerScript;

    [Header("Health")]
    public float maxHealth;
    private float health;
    public GameObject pnlHealthBar;
    private GameObject pnlHealthBarDisplay;
    private float initialHealthBarWidth;
    private float healthBarWidthIncrement;

    [Header("Armor")]
    public float totalArmorResistance;
    public float helmetArmorResistance;
    public float chestplateArmorResistance;
    public float leggingsArmorResistance;
    public float bootsArmorResistance;

    [Header("Attack")]
    public float damage;

    // Start is called before the first frame update
    void Start()
    {
        gameManagerScript = GameManagerScript.Instance;

        health = maxHealth;

        totalArmorResistance = 0;
        helmetArmorResistance = 0;
        chestplateArmorResistance = 0;
        leggingsArmorResistance = 0;
        bootsArmorResistance = 0;

        pnlHealthBarDisplay = pnlHealthBar.transform.GetChild(0).gameObject;
        initialHealthBarWidth = pnlHealthBarDisplay.GetComponent<RectTransform>().sizeDelta.x;
        healthBarWidthIncrement = initialHealthBarWidth / maxHealth;
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
}
