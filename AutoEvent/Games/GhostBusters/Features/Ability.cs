// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         Ability.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/28/2023 4:15 PM
//    Created Date:     10/28/2023 4:15 PM
// -----------------------------------------

using System;
using System.Collections.Generic;
using InventorySystem;
using InventorySystem.Items;
using PluginAPI.Core;

namespace AutoEvent.Games.GhostBusters.Features;

public class Ability
{
    public static Dictionary<ItemType, float> Damages = new Dictionary<ItemType, float>()
    {
        { ItemType.GrenadeHE, 800 },
    };
    public Ability(ItemType item, GhostBusterClassType classType, string description = "", float cooldown = 20, float initialCooldown = -2, short allowedUses = -1, float useDuration = 10f, Action<Player, Ability>? onAbilityUsed = null)
    {
        Description = description;
        Cooldown = cooldown;
        ClassType = classType;
        AllowedUses = allowedUses;
        ItemType = item;
        UseDuration = useDuration;
        
        if (initialCooldown == -2)
            InitialCooldown = cooldown;
        else
            InitialCooldown = initialCooldown;
        
        if (onAbilityUsed != null)
        {
            AbilityUsed += onAbilityUsed;
        }

        var itemInfo = createNewItem(ItemType);
        Serial = itemInfo.Key;
        ItemBase = itemInfo.Value;
    }
    public string Description { get; set; }
    public ItemType ItemType { get; set; }
    public ItemBase ItemBase { get; set; }
    public ushort Serial { get; set; }
    public short AllowedUses { get; set; }
    public GhostBusterClassType ClassType { get; set; }
    public float Cooldown { get; set; }
    public float InitialCooldown { get; set; }
    public float UseDuration { get; set; }
    public event Action<Player, Ability> AbilityUsed;
    public void OnAbilityUsed(Player ply, Ability ability) => AbilityUsed?.Invoke(ply, ability);
    
    private static KeyValuePair<ushort,ItemBase> createNewItem(ItemType item)
    {
        ushort serial = ItemSerialGenerator.GenerateNext();
        if(InventoryItemLoader.AvailableItems.TryGetValue(item, out var value) && value is not null)
        {
            value.ItemSerial = serial;
            KeyValuePair<ushort, ItemBase> kvp = new(serial, value);
            return kvp;
        }

        return new KeyValuePair<ushort, ItemBase>();
    }
}
