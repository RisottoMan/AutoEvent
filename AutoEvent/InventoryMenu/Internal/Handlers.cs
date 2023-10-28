// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          InventoryMenu
//    FileName:         Handlers.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/27/2023 4:22 PM
//    Created Date:     10/27/2023 4:22 PM
// -----------------------------------------

using InventoryMenu.API;
using InventoryMenu.API.EventArgs;
using InventoryMenu.API.Features;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;

namespace InventoryMenu.Internal;

internal class Handlers
{
    internal static Handlers Singleton { get; private set; }
    internal static void Init() => new Handlers();
    internal Handlers()
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (Singleton is not null)
            return;
        
        Singleton = this;
        EventManager.RegisterEvents(this);
    }

    [PluginEvent(ServerEventType.PlayerSearchPickup)]
    internal bool OnPickupItem(PlayerSearchPickupEvent ev)
    {
        var menu = MenuManager.Menus.FirstOrDefault(x => x.CanPlayerSee(ev.Player));
        return menu?.CanPlayersPickupItems ?? true;
    }
}