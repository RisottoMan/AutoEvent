using System.Collections.Generic;
using System.ComponentModel;
using AutoEvent.API;
using AutoEvent.Games.Battle;
using AutoEvent.Interfaces;
using PlayerRoles;
using UnityEngine;

namespace AutoEvent.Games.Infection
{
    public class BossConfig : EventConfig
    {
        [Description("A list of loadouts for non-boss players.")]
        public List<Loadout> Loadouts { get; set; } = new List<Loadout>()
        {
            new Loadout()
            {
                Chance = 33,
                Items = new List<ItemType>()
                {
                    ItemType.GunE11SR,
                    ItemType.Medkit,
                    ItemType.ArmorCombat,
                    ItemType.SCP1853,
                    ItemType.Adrenaline,
                },
                Health = 200,
                Roles = new Dictionary<RoleTypeId, int>() { { RoleTypeId.NtfSergeant, 100 } }
            },
            new Loadout()
            {
                Chance = 33,
                Items = new List<ItemType>()
                {
                    ItemType.GunShotgun,
                    ItemType.Medkit,
                    ItemType.Medkit,
                    ItemType.Medkit,
                    ItemType.Medkit,
                    ItemType.Medkit,
                    ItemType.Medkit,
                    ItemType.ArmorCombat,
                    ItemType.SCP500
                },
                Health = 200,
                Roles = new Dictionary<RoleTypeId, int>() { { RoleTypeId.NtfSpecialist, 100 } }
            },
            new Loadout()
            {
                Chance = 33,
                Items = new List<ItemType>()
                {
                    ItemType.GunLogicer,
                    ItemType.ArmorHeavy,
                    ItemType.SCP500,
                    ItemType.SCP500,
                    ItemType.SCP1853,
                    ItemType.Medkit,
                },
                Health = 200,
                ArtificialHealth = new ArtificialHealth(100),
                Roles = new Dictionary<RoleTypeId, int>() { { RoleTypeId.NtfCaptain, 100 } }
            }
        };

        [Description("A list of loadouts for boss players.")]
        public List<Loadout> BossLoadouts { get; set; } = new List<Loadout>()
        {
            new Loadout()
            {
                Roles = new Dictionary<RoleTypeId, int>() { { RoleTypeId.ChaosConscript, 100 } },
                Size = new Vector3(5, 5, 5),
                Items = new List<ItemType>() { ItemType.GunLogicer, ItemType.Ammo556x45 }
            },
            
        };

        [Description("How many players should be on the boss team. [Default: 1 Player]")]
        public RoleCount BossCount { get; set; } = new RoleCount()
        {
            MinimumPlayers = 1, // at least one player
            MaximumPlayers = 1, // only one player
            PlayerPercentage = -1 // ignore percentage of players ingame.
        };

        [Description("How long the event should last in seconds. [Default: 120]")]
        public int DurationInSeconds { get; set; } = 120;
    }
}