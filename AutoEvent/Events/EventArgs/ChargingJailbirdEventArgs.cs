// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         ChargingJailbirdEventArgs.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/15/2023 7:10 PM
//    Created Date:     10/15/2023 7:10 PM
// -----------------------------------------

using Exiled.API.Features.Items;
using InventorySystem.Items;
using PluginAPI.Core;

namespace AutoEvent.Events.EventArgs;

/// <summary>
/// Contains all information before a player charges a <see cref="Jailbird"/>.
/// </summary>
public class ChargingJailbirdEventArgs 
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChargingJailbirdEventArgs"/> class.
    /// </summary>
    /// <param name="player"><inheritdoc cref="Player"/></param>
    /// <param name="swingItem">The item being charged.</param>
    /// <param name="isAllowed">Whether the item can be charged or not.</param>
    public ChargingJailbirdEventArgs(ReferenceHub player, InventorySystem.Items.ItemBase swingItem, bool isAllowed = true)
    {
        Player = Player.Get(player);
        Item = swingItem;
        IsAllowed = isAllowed;
    }

    /// <summary>
    /// Gets the <see cref="API.Features.Player"/> who's charging an item.
    /// </summary>
    public Player Player { get; }

    /// <summary>
    /// Gets the <see cref="API.Features.Items.Item"/> that is being charged. This will always be a <see cref="Jailbird"/>.
    /// </summary>
    public ItemBase Item { get; }

    /// <summary>
    /// Gets or sets a value indicating whether or not the Jailbird can be charged.
    /// </summary>
    public bool IsAllowed { get; set; }
}