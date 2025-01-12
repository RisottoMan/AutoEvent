using System.Collections.Generic;
using System.ComponentModel;
using AutoEvent.API;
using AutoEvent.API.Season.Enum;
using AutoEvent.Interfaces;
using Exiled.API.Enums;
using PlayerRoles;
using UnityEngine;

namespace AutoEvent.Games.Airstrike;

[Description("Be aware that this plugin can cause lag if not carefully balanaced.")]
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
            AvailableMaps.Add(new MapChance(50, new MapInfo("DeathParty", new Vector3(0f, 40f, 0f))));
            AvailableMaps.Add(new MapChance(50, new MapInfo("DeathParty_Xmas2024", new Vector3(0f, 40f, 0f)), SeasonFlags.Christmas));
        }
    }
    
    [Description("Should grenades spawn on top of randomly chosen players. This will not apply on the last round.")]
    public bool TargetPlayers { get; set; } = false;

    [Description("If true, players will respawn as chaos, and get to lob grenades at people who are still alive.")]
    public bool RespawnPlayersWithGrenades { get; set; } = false;

    [Description("The amount of rounds that this gamemode lasts. The last round is always a super big grenade.")] 
    public int Rounds { get; set; } = 5;

    [Description("If enabled the minigame will end when there is only one player left. Otherwise it will end when everyone dies, or the rounds (configurable) are over.")] 
    public bool LastPlayerAliveWins { get; set; } = true;
    
    [Description("A list of loadouts.")]
    public List<Loadout> Loadouts { get; set; } = new()
    {
        new Loadout()
        {
            Roles = new Dictionary<RoleTypeId, int>()
            {
                { RoleTypeId.ClassD, 100 },
            },
            Effects = new() { new (EffectType.FogControl, 0) }
        }
    };
    
    [Description("A list of failure loadouts.")]
    public List<Loadout> FailureLoadouts { get; set; } = new()
    {
        new Loadout()
        {
            Roles = new Dictionary<RoleTypeId, int>()
            {
                { RoleTypeId.ChaosConscript, 100 },
            },
            Effects = new() { new (EffectType.FogControl, 0) }
        }
    };
}