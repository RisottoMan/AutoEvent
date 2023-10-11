// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         Item.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/02/2023 10:08 PM
//    Created Date:     10/02/2023 10:08 PM
// -----------------------------------------

using System.ComponentModel;
using YamlDotNet.Serialization;

namespace AutoEvent.API;

public class Item
{
    public static Item GetOrCreate(ItemType item)
    {
        if (item.IsWeapon())
        {
            return new Weapon(item);
        }

        return new Item(item);
    }
    public Item()
    {
    }
    public Item(ItemType itemType)
    {
        ItemType = itemType;
    }
    
    [Description("The item type.")]
    public ItemType ItemType { get; set; }
    
    public override bool Equals(object obj)
    {
        if (obj is ItemType type)
        {
            if (type == ItemType)
                return true;
            return false;
        }

        return false;
    }

    public override string ToString()
    {
        return $"{ItemType}";
    }
}