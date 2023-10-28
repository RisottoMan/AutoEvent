// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          InventoryMenu
//    FileName:         Menu.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/27/2023 2:17 PM
//    Created Date:     10/27/2023 2:17 PM
// -----------------------------------------

using System.Collections.ObjectModel;
using HarmonyLib;
 using InventoryMenu.API;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Pickups;
using PluginAPI.Core;
using PluginAPI.Core.Items;

namespace InventoryMenu.API.Features;

public sealed class Menu
{
    private static int Index { get; set; } = 0;
    public static Menu? GetMenu(int id)
    {
        return MenuManager.Menus.FirstOrDefault(x => x.Id == id);
    }
    public Menu(string description, Dictionary<byte, MenuItem>? items = null)
    {
        this.Id = Index;
        Index++;
        this.Description = description;
        this._playerInventoryCaches = new Dictionary<Player, PlayerInventoryCache>();
        this._items = items ?? new Dictionary<byte, MenuItem>();
        MenuManager.RegisterMenu(this);
    }
    /// <summary>
    /// The <see cref="Id"/> of the menu. Unique for every menu.
    /// </summary>
    public int Id { get; private set; } 
    
    /// <summary>
    /// A description of the menu.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets or Sets whether a <see cref="Player"/> can pickup an item.
    /// </summary>
    /// <returns>True if the player can pickup an item. False if a player cannot pickup items.</returns>
    public bool CanPlayersPickupItems { get; set; } = false;
    
    /// <summary>
    /// An internal dictionary of menu items corresponding to their position.
    /// </summary>
    internal Dictionary<byte, MenuItem> _items { get; set; }

    /// <summary>
    /// A read-only dictionary of menu items and their corresponding positions.
    /// </summary>
    public ReadOnlyDictionary<byte, MenuItem> Items => new(_items);

    /// <summary>
    /// Adds a <see cref="MenuItem"/> to the menu.
    /// </summary>
    /// <param name="item">The <see cref="MenuItem"/> to add.</param>
    /// <returns>True if the item can be added, False if the item cannot be added.</returns>
    public bool AddItem(MenuItem item)
    {
        if (_items.Count >= 8)
            return false;

        byte position = (item.CachedPosition == 255) ? (byte)0 : item.CachedPosition;
        
        // Something already exists there.
        if (_items.ContainsKey(item.CachedPosition))
        {
            for (byte i = 0; i < 8; i++)
            {
                if (_items.ContainsKey(i))
                    continue;
                position = i;
                break;
            }
        }

        item.CachedPosition = 255;
        _items.Add(position, item);
        return true;
    }

    public bool RemoveItem(MenuItem item)
    {
        if (!_items.ContainsValue(item))
        {
            return false;
            // dont try to remove it
        }
        byte key = _items.FirstOrDefault(x => x.Value == item).Key;
        _items.Remove(key);
        item.AssignParentMenu(null);
        return true;
    }

    public bool ChangeItemIndex(byte oldPosition, byte newPosition)
    {
        // Old position doesnt exist.
        if (!_items.ContainsKey(oldPosition))
            return false;
        
        // Not moving anything.
        if (oldPosition == newPosition)
            return true;

        // Something already exists there.
        if (_items.ContainsKey(newPosition))
            return false;
        
        // Add the new index.
        _items.Add(newPosition, _items[oldPosition]);
        // Remove the old index.
        _items.Remove(oldPosition);
        return true;
    }

    private Dictionary<Player, PlayerInventoryCache> _playerInventoryCaches { get; set; }
    
    /// <summary>
    /// The instances of players viewing this menu.
    /// </summary>
    public ReadOnlyDictionary<Player, PlayerInventoryCache> PlayerInventoryCaches => new(_playerInventoryCaches);

    /// <summary>
    /// Checks if a player is being shown the menu.
    /// </summary>
    /// <param name="ply">The name of the player.</param>
    /// <returns></returns>
    public bool CanPlayerSee(Player ply)
    {
        return _playerInventoryCaches.ContainsKey(ply);
    }
    
    /// <summary>
    /// Shows a menu to a player.
    /// </summary>
    /// <param name="ply">The <see cref="Player"/> to show the menu to.</param>
    public void ShowToPlayer(Player ply)
    {
        if (_playerInventoryCaches.ContainsKey(ply))
            return;
        
        _playerInventoryCaches.Add(ply, ply.StoreAndClearInventory());
        ply.ReferenceHub.inventory.SendItemsNextFrame = true;
    }

    /// <summary>
    /// Hides the menu for a player. 
    /// </summary>
    /// <param name="ply">The <see cref="Player"/> to hide the menu from.</param>
    public void HideForPlayer(Player ply)
    {
        if (!_playerInventoryCaches.ContainsKey(ply))
            return;
        
        _playerInventoryCaches[ply].RestoreInventory();
        _playerInventoryCaches.Remove(ply);
    }

    /// <summary>
    /// Adds pickups to the "internal" inventory instead.
    /// </summary>
    /// <param name="ply">The <see cref="Player"/> picking up the item.</param>
    /// <param name="item">The item the player is receiving.</param>
    internal void ProcessPickup(Player ply, ItemPickupBase item)
    {
        /*
        ushort serial = item.Info.Serial;
        //if (ply.ReferenceHub.inventory.UserInventory.Items.Count >= 8 && InventoryItemLoader.AvailableItems.TryGetValue(item.Info.ItemId, out var value) && value.Category != ItemCategory.Ammo)
        if (!this._playerInventoryCaches.ContainsKey(ply))
        {
            var cache = new PlayerInventoryCache(ply);
            cache.ForceClearedInventory(true);
            _playerInventoryCaches.Add(ply, cache);
        }
        if(this._playerInventoryCaches[ply].Items.Count >= 8 && InventoryItemLoader.AvailableItems.TryGetValue(item.Info.ItemId, out var value) && value.Category != ItemCategory.Ammo)
        {
            goto itemSkip;
        }
        if (serial == 0)
        {
            serial = ItemSerialGenerator.GenerateNext();
        }
        ItemBase itemBase = ply.ReferenceHub.inventory.CreateItemInstance(new ItemIdentifier(item.Info.ItemId, serial), ply.ReferenceHub.inventory.isLocalPlayer);
        if (itemBase == null)
        {
            goto itemSkip;
        }
        
        // ply.ReferenceHub.inventory.UserInventory.Items[serial] = itemBase;
        _playerInventoryCaches[ply].Items[serial] = itemBase;
        
        itemBase.OnAdded(item);
        try
        {
            typeof(InventoryExtensions).GetEvents().First(x => x.Name == nameof(InventoryExtensions.OnItemAdded))
                .RaiseMethod.Invoke(null, new object[] { ply.ReferenceHub, itemBase, item });
        }
        catch (Exception e)
        {
            Log.Error($"Could not raise Event InventoryExtensions.OnItemAdded. This will call it manually, however it may break plugins and exiled.");
            Log.Debug($"Exception at Menu.ProcessPickup => InventoryExtensions.OnItemAdded.Invoke(). Exception: \n{e}");
            
            Respawning.ItemPickupTokens.OnItemAdded(ply.ReferenceHub, itemBase, item);
            Achievements.Handlers.ItemPickupHandler.OnItemAdded(ply.ReferenceHub, itemBase, item);
        }
        if (ply.ReferenceHub.inventory.isLocalPlayer && itemBase is IAcquisitionConfirmationTrigger acquisitionConfirmationTrigger)
        {
            acquisitionConfirmationTrigger.ServerConfirmAcqusition();
            acquisitionConfirmationTrigger.AcquisitionAlreadyReceived = true;
        }
        //ply.ReferenceHub.inventory.SendItemsNextFrame = true;
        
        itemSkip:
        item.DestroySelf();
        */
    }

}