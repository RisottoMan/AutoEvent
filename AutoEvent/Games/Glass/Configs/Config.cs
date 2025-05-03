using System.Collections.Generic;
using System.ComponentModel;
using AutoEvent.API;
using AutoEvent.API.Season.Enum;
using AutoEvent.Interfaces;
using Exiled.API.Enums;
using Exiled.API.Features;
using PlayerRoles;
using UnityEngine;

namespace AutoEvent.Games.Glass;

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
            AvailableMaps.Add(new MapChance(50, new MapInfo("Glass", new Vector3(0, 40f, 0f), null, null, false)));
            AvailableMaps.Add(new MapChance(50, new MapInfo("Glass_Xmas2024", new Vector3(0, 40f, 0f), null, null, false), SeasonFlags.Christmas));
        }
    }

    [Description("Random percent from 40 - 60 will be chosen (ex: 50 percent). 50% of the platforms will be on one side. 50% on the other. This is the Lower Value.")]
    public int MinimumSideOffset { get; set; } = 40;
    [Description("Random percent from 40 - 60 will be chosen (ex: 50 percent). 50% of the platforms will be on one side. 50% on the other. This is the Higher Value.")]
    public int MaximumSideOffset { get; set; } = 60;

    [Description("The salt to use for the cryptographic seed generator.")]
    public string SeedSalt { get; set; } = "salty hasbrown";
    
    [Description("How many seconds until the platforms regenerate. Prevents players from sitting around. -1 to disable.")]
    public float BrokenPlatformRegenerateDelayInSeconds { get; set; } = 5;
    [Description("The players will push each other for fun. Default: false.")]
    public bool IsEnablePush { get; set; } = true;

    [Description("How much time should I give the player in seconds to cool down to use the push?")]
    public float PushPlayerCooldown { get; set; } = 5;
    
    [Description("The loadouts a player can get.")]
    public List<Loadout> Loadouts { get; set; } = new()
    {
        new Loadout()
        {
            Roles = new Dictionary<RoleTypeId, int>()
            {
                { RoleTypeId.ClassD, 100 },
            },
            Effects = new List<Effect>() { new(EffectType.FogControl, 0) },
        }
    };
}