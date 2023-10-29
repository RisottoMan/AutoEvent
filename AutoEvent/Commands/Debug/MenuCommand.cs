// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         Menu.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/27/2023 6:33 PM
//    Created Date:     10/27/2023 6:33 PM
// -----------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using AutoEvent.Interfaces;
using CommandSystem;
using InventoryMenu.API.Features;
using InventoryMenu.API;
using InventoryMenu;
using InventoryMenu.API.EventArgs;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Attachments;
using MEC;
using PluginAPI.Core;
using Utils.NonAllocLINQ;

namespace AutoEvent.Commands.Debug;

public class MenuCommand : ICommand, IPermission
{
    public string Permission { get; set; } = "ev.debug";
    public string Command => "Menu";
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, [UnscopedRef] out string response)
    {
        if (!sender.CheckPermission(Permission, out bool isConsole))
        {
            response = "You don't have permission to use this command.";
            return false;
        }

        if (arguments.Count < 1)
        {
            response = "You must specify a player";
            return false;
        }
        var player = Player.GetPlayers().FirstOrDefault(ply => ply.Nickname.ToLower().Contains(arguments.At(0).ToLower()));
        if (player is null)
        {
            response = $"Could not find player \"{arguments.At(0)}\"";
            return false;
        }

        if (arguments.Count >= 2 && arguments.At(1).ToLower() is "demo")
            goto demo;
        bool hide = arguments.Count >= 2 && arguments.At(1).ToLower() is "true" or "1" or "hide";
        bool canPickupItems = arguments.Count >= 3 && arguments.At(2).ToLower() is "true" or "1" or "pickup";
        if (hide)
        {
            player.HideMenu();
            response = $"Hid menu for player {player.Nickname}";
            return true;
        }
        
        Menu menu = new Menu("Test menu.", canPickupItems);
        menu.AddItem(new MenuItem(ItemType.Coin, "test coin", 0, OnRecieved));
        menu.AddItem(new MenuItem(ItemType.KeycardScientist, "test keycard", 1, OnRecieved));
        menu.AddItem(new MenuItem(ItemType.SCP018, "test ball", 2,OnRecieved));
        menu.AddItem(new MenuItem(ItemType.GrenadeHE, "test grenade", 3, OnRecieved));
        menu.AddItem(new MenuItem(ItemType.SCP330, "test candy", 4, OnRecieved));
        player.ShowMenu(menu);
        
        response = "Menu ";
        return true;
        demo:
        Menu demo = new Menu("Test Menu.", false);
        demo.AddItem(new MenuItem(ItemType.GunE11SR, "The AK Loadout.", 0, OnRecievedLoadout));
        demo.AddItem(new MenuItem(ItemType.GunLogicer, "The Logicer Loadout.", 1, OnRecievedLoadout));
        demo.AddItem(new MenuItem(ItemType.GunShotgun, "The Shotgun Loadout.", 2, OnRecievedLoadout));
        player.ShowMenu(demo);
        player.SendBroadcast(demoBroadcast, 1200);
        response = "Gave player demo menu selection.";
        return true;
    }

    private static string demoBroadcast = "Select a loadout in your inventory to spawn with.";
    private static void OnRecievedLoadout(MenuItemClickedArgs ev)
    {
        if (!ev.IsLeftClick)
        {
            ushort bcDelay = 5;
            ev.Player.ClearBroadcasts();
            ev.Player.SendBroadcast($"Loadout Option: {ev.MenuItemClicked.Description}", bcDelay);
            Timing.CallDelayed(bcDelay, () => { ev.Player.SendBroadcast(demoBroadcast, 1200 );});
            return;
        }

        switch (ev.MenuItemClicked.Item)
        {
            case ItemType.GunE11SR:
                ev.Player.AddItem(ItemType.GunE11SR);
                ev.Player.AddItem(ItemType.ArmorCombat);
                ev.Player.AddItem(ItemType.Painkillers);
                ev.Player.AddItem(ItemType.SCP207);
                ev.Player.AddItem(ItemType.Medkit);
                    //weapon.ApplyAttachmentsCode((uint)AttachmentName.Flashlight);
                break;
            case ItemType.GunLogicer:
                ev.Player.AddItem(ItemType.GunLogicer);
                ev.Player.AddItem(ItemType.ArmorHeavy);
                ev.Player.AddItem(ItemType.Medkit);
                ev.Player.AddItem(ItemType.Painkillers);
                break;
            case ItemType.GunShotgun:
                ev.Player.AddItem(ItemType.GunShotgun);
                ev.Player.AddItem(ItemType.GunRevolver);
                ev.Player.AddItem(ItemType.ArmorCombat);
                ev.Player.AddItem(ItemType.GrenadeHE);
                ev.Player.AddItem(ItemType.GrenadeHE);
                ev.Player.AddItem(ItemType.Medkit);
                break;
        }
        ev.Player.HideMenu();
    }
    private static void OnRecieved(MenuItemClickedArgs ev)
    {
        DebugLogger.LogDebug($"{ev.MenuItemClicked.Item} [{(ev.IsLeftClick ? "Select" :  "Drop")}]");
    }

    public string[] Aliases => Array.Empty<string>();
    public string Description => "Provides a test menu to a player.";
}