using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Health")]
    public float health;
    public float maxHealth;

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
        totalArmorResistance = 0;
        helmetArmorResistance = 0;
        chestplateArmorResistance = 0;
        leggingsArmorResistance = 0;
        bootsArmorResistance = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
