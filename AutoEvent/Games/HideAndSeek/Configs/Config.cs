using System.Collections.Generic;
using System.ComponentModel;
using AutoEvent.API;
using AutoEvent.API.Season.Enum;
using AutoEvent.Interfaces;
using Exiled.API.Enums;
using Exiled.API.Features;
using PlayerRoles;
using UnityEngine;

namespace AutoEvent.Games.HideAndSeek;
public class Config : EventConfig
{
    public Config()
    {
        if (AvailableMaps is null)
        {
            AvailableMaps = new List<MapChance>();
        }

        if (AvailableMaps.Count < 1)
        {
            AvailableMaps.Add(new MapChance(50, new MapInfo("HideAndSeek", new Vector3(0, 30, 30))));
            AvailableMaps.Add(new MapChance(50, new MapInfo("HideAndSeek_Xmas2024", new Vector3(0, 30, 30)), SeasonFlags.Christmas));
            AvailableMaps.Add(new MapChance(50, new MapInfo("HideAndSeek_Halloween2024", new Vector3(0, 30, 30)), SeasonFlags.Halloween));
        }
    }

    [Description("The item that the tagged player should get. Do not do Scp018 or Grenades for now. - They will break the event. (working on it - redforce)")]
    public ItemType TaggerWeapon { get; set; } = ItemType.Jailbird;
    
    [Description("Players who are not the tagger will have the breach scanner effect applied to them, when there are less than or equal to this many non-taggers alive.")]
    public int PlayersRequiredForBreachScannerEffect { get; set; } = 2;

    [Description("How long should the tagger get immunity.")]
    public float NoTagBackDuration { get; set; } = 3f;

    [Description("How long players have to tag someone else.")]
    public int TagDuration { get; set; } = 30;
    
    [Description("How long players have to rest before the next tagger group is selected.")]
    public int BreakDuration { get; set; } = 15;

    [Description("The amount of taggers that should spawn.")]
    public RoleCount TaggerCount { get; set; } = new RoleCount(1, 6, 35);
    
    [Description("Can be used to disable the jailbird charging attack.")]
    public bool JailbirdCanCharge { get; set; } = false;

    [Description("A list of loadouts players can get.")]
    public List<Loadout> PlayerLoadouts { get; set; } = new()
    {
        new Loadout()
        {
            Roles = new Dictionary<RoleTypeId, int>() { { RoleTypeId.ClassD, 100 } },
            Effects = new List<Effect>()
            {
                new(EffectType.MovementBoost, 0, 50),
                new(EffectType.FogControl, 0),
            },
            Chance = 100,
            InfiniteAmmo = AmmoMode.InfiniteAmmo
        }
    };

    public List<Loadout> TaggerLoadouts { get; set; } = new()
    {
        new Loadout()
        {
            Roles = new Dictionary<RoleTypeId, int>() { { RoleTypeId.Scientist, 100 } },
            Effects = new List<Effect>() 
            { 
                new(EffectType.MovementBoost, 0, 70),
                new(EffectType.FogControl, 0),
            },
            Chance = 100,
            InfiniteAmmo = AmmoMode.InfiniteAmmo
        }
    };
}