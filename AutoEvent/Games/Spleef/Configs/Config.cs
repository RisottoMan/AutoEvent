using System.Collections.Generic;
using System.ComponentModel;
using AutoEvent.API;
using AutoEvent.Interfaces;
using PlayerRoles;

namespace AutoEvent.Games.Spleef;

public class Config : EventConfig
{
    [Description("How long the round should last.")]
    public int RoundDurationInSeconds { get; set; } = 300;

    [Description("Set to -1 to disable. Causes platforms to regenerate after a certain number of seconds.")]
    public int RegeneratePlatformsAfterXSeconds { get; set; } = -1;

    [Description("How many platforms on the x and y axis. Total Platforms = PlatformAxisCount * PlatformAxisCount * LayerCount. Size is automatically determined.")]
    public int PlatformAxisCount { get; set; } = 20;
    [Description("How many \"levels\" of height should be in the map. ")]
    public int LayerCount { get; set; } = 4;

    [Description("How long before platforms will fall after being stepped on. Set to -1 to disable this.")]
    public float PlatformFallDelay { get; set; } = -1;

    [Description("The amount of health platforms have. Set to -1 to make them invincible.")]
    public float PlatformHealth { get; set; } = 1;

    [Description("A list of loadouts for spleef if a little count of players.")]
    public List<Loadout> PlayerLittleLoadouts { get; set; } = new List<Loadout>()
    {
        new Loadout()
        {
            Roles = new Dictionary<RoleTypeId, int>()
            {
                { RoleTypeId.ClassD, 100 },
            },
            Items = new List<ItemType>()
            {
                ItemType.GunCom45,
            },
            InfiniteAmmo = AmmoMode.InfiniteAmmo
        }
    };

    [Description("A list of loadouts for spleef if a normal count of players.")]
    public List<Loadout> PlayerNormalLoadouts { get; set; } = new List<Loadout>()
    {
        new Loadout()
        {
            Roles = new Dictionary<RoleTypeId, int>()
            {
                { RoleTypeId.ClassD, 100 },
            },
            Items = new List<ItemType>()
            {
                ItemType.GunCOM18,
            },
            InfiniteAmmo = AmmoMode.InfiniteAmmo
        }
    };

    [Description("A list of loadouts for spleef if a big count of players.")]
    public List<Loadout> PlayerBigLoadouts { get; set; } = new List<Loadout>()
    {
        new Loadout()
        {
            Roles = new Dictionary<RoleTypeId, int>()
            {
                { RoleTypeId.ClassD, 100 },
            },
            Items = new List<ItemType>()
            {
                ItemType.GunCOM15,
            },
            InfiniteAmmo = AmmoMode.InfiniteAmmo
        }
    };
}