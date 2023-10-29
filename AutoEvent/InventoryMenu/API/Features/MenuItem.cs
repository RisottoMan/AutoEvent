// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          InventoryMenu
//    FileName:         MenuItem.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/27/2023 2:09 PM
//    Created Date:     10/27/2023 2:09 PM
// -----------------------------------------

using InventoryMenu.API.EventArgs;
using InventorySystem.Items;
using PluginAPI.Core;

namespace InventoryMenu.API.Features;

public sealed class MenuItem
{
    /// <summary>
    /// Used to track items.
    /// </summary>
    private static int Index { get; set; } = 0;
    
    public MenuItem(ItemType item, string description, byte position = 255, Action<MenuItemClickedArgs>? onClicked = null, Menu? parent = null, ItemBase? itemBase = null)
    {
        CachedPosition = position;
        
        Id = Index;
        Index++;
        
        this.Item = item;
        Serial = 0;
        if (itemBase is not null)
        {
            ItemBase = itemBase;   
            Serial = itemBase.ItemSerial;
        }
        this.Description = description;
        if (onClicked is not null)
        {
            Clicked += onClicked;
        }

        if (parent is not null)
        {
            ParentMenu = parent;
        }
    }
    
    public int Id { get; private set; } 
    
    /// <summary>
    /// The item type of the selection.
    /// </summary>
    public ItemType Item { get; private set; }
    
    internal ItemBase? ItemBase { get; set; }
    
    /// <summary>
    /// The serial of the item.
    /// </summary>
    public ushort Serial { get; internal set; }
    
    /// <summary>
    /// The description shown to players for the item.
    /// </summary>
    public string Description { get; set; }
    
    /// <summary>
    /// The position the item should be in. (Between 0 and 7)
    /// </summary>
    public byte ItemPosition {
        get
        {
            if (ParentMenu == null)
                return CachedPosition == 255 ? (byte) 0 : CachedPosition;
            return ParentMenu.Items.First(x => x.Value == this).Key;
        }
        set => this.ChangePosition(value);
    }

    internal byte CachedPosition { get; set; } = 255;
    
    /// <summary>
    /// Triggered when a player selects this menu item.
    /// </summary>
    public event Action<MenuItemClickedArgs> Clicked;
    
    /// <summary>
    /// Invokes <see cref="Clicked"/>
    /// </summary>
    /// <param name="ev">The parameters to invoke.</param>
    internal void OnClicked(MenuItemClickedArgs ev) => Clicked?.Invoke(ev);
    
    /// <summary>
    /// The parent <see cref="Menu"/> for this <see cref="MenuItem"/>. Can be false if a parent hasn't been defined.
    /// </summary>
    public Menu? ParentMenu { get; private set; }
    
    /// <summary>
    /// Changes the position that this item will appear in, in the inventory.
    /// </summary>
    /// <param name="newPosition">The new position.</param>
    /// <returns>True if the item can change positions. False if another item is already using the position.</returns>
    public bool ChangePosition(byte newPosition)
    {
        if (ParentMenu is null)
        {
            CachedPosition = ItemPosition;
            return false;
        }

        return ParentMenu.ChangeItemIndex(ItemPosition, newPosition);
    }

    
    /// <summary>
    /// Assigns this item to a parent menu.
    /// </summary>
    /// <param name="menu"></param>
    /// <exception cref="ArgumentException"></exception>
    internal void AssignParentMenu(Menu? menu)
    {
        if (menu == null)
        {
            if (ParentMenu is not null && ParentMenu.Items.Any(x => x.Value == this))
            {
                int parentId = ParentMenu.Id;
                ParentMenu = null;
                Menu.GetMenu(parentId)?.RemoveItem(this);
            }
            return;
        }
        if (menu.Items.Any(x => x.Value == this))
        {
            return;
        }
        if (menu.Items.Count > 7)
        {
            throw new ArgumentException("The parent menu has too many items.");
        }
        ParentMenu = menu;
        ParentMenu.AddItem(this);
    }
    
}