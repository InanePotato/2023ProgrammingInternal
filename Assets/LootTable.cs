using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Loot Table", menuName = "Loot Table")]
public class LootTable : ScriptableObject
{
    public List<LootTableItem> lootTable = new List<LootTableItem>();

    [Serializable]
    public struct LootTableItem
    {
        public Item item;
        public int minAmmountPerDrop;
        public int maxAmmountPerDrop;
        public int maxTimesDropable;
        public int rarity;
    }
}
