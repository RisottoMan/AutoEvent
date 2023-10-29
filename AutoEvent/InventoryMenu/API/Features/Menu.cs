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
using InventoryMenu.API.EventArgs;
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
    public Menu(string description, bool canPickupItems = false, Action<GetMenuItemsForPlayerArgs>? onGetMenuItems = null, Dictionary<byte, MenuItem>? items = null)
    {
        this.Id = Index;
        Index++;
        this.CanPlayersPickupItems = canPickupItems;
        this.Description = description;
        this._activePlayers = new List<Player>();
        this._items = items ?? new Dictionary<byte, MenuItem>();
        if (onGetMenuItems is not null)
        {
            GetMenuItems += onGetMenuItems;
        }
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

    public event Action<GetMenuItemsForPlayerArgs> GetMenuItems;
    public void OnGetMenuItems(GetMenuItemsForPlayerArgs ev) => GetMenuItems?.Invoke(ev);
    
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
        if (_items.ContainsKey(position))
        {
            for (byte i = 0; i < 8; i++)
            {
                if (_items.ContainsKey(i))
                    continue;
                position = i;
                break;
            }
        }
        
        _items.Add(position, item);
        
        item.CachedPosition = 255;
        if (item.Serial != 0)
        {
            //Log.Debug($"[Menu {this.Id}] Reusing Serial for Item {item.Id} ({item.Item}, {item.Serial})");
            this._itemBases.Add(item.Serial, item.ItemBase);
            return true;
        }
        ushort serial = ItemSerialGenerator.GenerateNext();
        if(InventoryItemLoader.AvailableItems.TryGetValue(item.Item, out var value) && value is not null)
        {
            //Log.Debug($"[Menu {this.Id}] Generating Serial for Item {item.Id} ({item.Item}, {item.Serial})");
            item.Serial = serial;
            
            this._itemBases.Add(serial, value);
            return true;
        }
        // ItemBase itemBase = ply.ReferenceHub.inventory.CreateItemInstance(new ItemIdentifier(item.Info.ItemId, serial), ply.ReferenceHub.inventory.isLocalPlayer);
        return true;
    }

    public bool RemoveItem(MenuItem item)
    {
        if (!_items.ContainsValue(item))
        {
            return false;
            // dont try to remove it
        }

        if (_itemBases.ContainsKey(item.Serial))
            _itemBases.Remove(item.Serial);
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

    private List<Player> _activePlayers { get; set; }
    
    /// <summary>
    /// The instances of players viewing this menu.
    /// </summary>
    public IReadOnlyList<Player> PlayerInventoryCaches => _activePlayers.AsReadOnly();

    /// <summary>
    /// Checks if a player is being shown the menu.
    /// </summary>
    /// <param name="ply">The name of the player.</param>
    /// <returns></returns>
    public bool CanPlayerSee(Player ply)
    {
        return _activePlayers.Contains(ply);
    }

    /// <summary>
    /// Shows a menu to a player.
    /// </summary>
    /// <param name="ply">The <see cref="Player"/> to show the menu to.</param>
    public void ShowToPlayer(Player ply)
    {
        if (_activePlayers.Contains(ply))
            return;
        ply.HideMenu();
        _activePlayers.Add(ply);

        ply.ReferenceHub.inventory.SendItemsNextFrame = true;
    }
    
    /// <summary>
    /// Hides the menu for a player. 
    /// </summary>
    /// <param name="ply">The <see cref="Player"/> to hide the menu from.</param>
    public void HideForPlayer(Player ply)
    {
        if (!_activePlayers.Contains(ply))
            return;
        
        _activePlayers.Remove(ply);
        ply.ReferenceHub.inventory.SendItemsNextFrame = true;

    }

    public Dictionary<ushort, ItemBase> _itemBases { get; set; } = new Dictionary<ushort, ItemBase>();
    public ReadOnlyDictionary<ushort, ItemBase> ItemBases => new(_itemBases);
}