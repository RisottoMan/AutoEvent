// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          InventoryMenu
//    FileName:         MenuManager.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/27/2023 3:08 PM
//    Created Date:     10/27/2023 3:08 PM
// -----------------------------------------

using HarmonyLib;
using InventoryMenu.API.Features;
using PluginAPI.Core.Attributes;

namespace InventoryMenu.API;

public sealed class MenuManager
{
    static MenuManager()
    {
        _menus = new List<Menu>();
    }   
    
    /// <summary>The main instance of Harmony.</summary>
    internal static Harmony Harmony { get; set; }
    
    /// <summary>
    /// Where all registered menus are stored.
    /// </summary>
    public static IReadOnlyList<Menu> Menus => _menus.AsReadOnly();
    
    /// <summary>
    /// The modifiable collection holding the instances of menus.
    /// </summary>
    private static List<Menu> _menus { get; set; }
    
    /// <summary>
    /// The public singleton instance of the menu manager.
    /// </summary>
    public static MenuManager Singleton { get; private set; }
    
    /// <summary>
    /// Initializes the <see cref="MenuManager"/>. This is required for the menu system to work.
    /// </summary>
    [PluginEntryPoint("Inventory Menus", "v1.0.1", "Provides Inventory Menus", "Redforce04")]
    public static void Init() => new MenuManager();
    
    /// <summary>
    /// Registers a menu.
    /// </summary>
    /// <param name="menu">The menu to register.</param>
    internal static void RegisterMenu(Menu menu)
    {
        if(!_menus.Contains(menu))
            _menus.Add(menu);
    }
    
    /// <summary>
    /// Initializes the menu manager.
    /// </summary>
    public MenuManager()
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (Singleton is not null)
            return;
        
        Singleton = this;
        CosturaUtility.Initialize();
        InventoryMenu.Internal.Handlers.Init();
        Harmony = new Harmony("me.redforce04.inventorymenus");
        Harmony.PatchAll();
        Log.Debug("Inventory Manager Initialized.");
    }

    


}
