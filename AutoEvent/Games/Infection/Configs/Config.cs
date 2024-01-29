using System;
using System.Collections.Generic;
using System.ComponentModel;
using AutoEvent.API;
using AutoEvent.Interfaces;
using PlayerRoles;
using UnityEngine;

namespace AutoEvent.Games.Infection
{
    public class InfectConfig : EventConfig
    {
        public InfectConfig()
        {
            if (AvailableMaps is null)
            {
                AvailableMaps = new List<MapChance>();
            }

            if (AvailableSounds is null)
            {
                AvailableSounds = new List<SoundChance>();
            }

            if (AvailableMaps.Count < 1)
            {
                AvailableMaps.Add(new MapChance(50, new MapInfo("Zombie", new Vector3(115.5f, 1030f, -43.5f) )));
                AvailableMaps.Add(new MapChance(50, new MapInfo("ZombieRework", new Vector3(115.5f, 1030f, -43.5f))));
                AvailableMaps.Add(new MapChance(50, new MapInfo("Zombie_Xmas2024", new Vector3(115.5f, 1030f, -43.5f))));
                AvailableMaps.Add(new MapChance(50, new MapInfo("ZombieRework_Xmas2024", new Vector3(115.5f, 1030f, -43.5f))));
            }

            if (AvailableSounds.Count < 1)
            {
                AvailableSounds.Add(new SoundChance(33, new SoundInfo("Zombie.ogg", 7)));
                AvailableSounds.Add(new SoundChance(33, new SoundInfo("Zombie2.ogg", 7)));
                AvailableSounds.Add(new SoundChance(33, new SoundInfo("Zombie_Run.ogg", 7)));
            }
        }

        public List<Loadout> PlayerLoadouts { get; set; } = new List<Loadout>()
        {
            new Loadout()
            {
                Roles = new Dictionary<RoleTypeId, int>() { { RoleTypeId.ClassD, 100 } }
            }
        };

        public List<Loadout> ZombieLoadouts { get; set; } = new List<Loadout>()
        {
            new Loadout()
            {
                Roles = new Dictionary<RoleTypeId, int>() { { RoleTypeId.Scp0492, 100 } }
            }
        };
        /* // Christmas Update
        public List<Loadout> FlamingoLoadouts { get; set; } = new List<Loadout>()
        {
            new Loadout()
            {
                Roles = new Dictionary<RoleTypeId, int>() 
                {
                    { RoleTypeId.Flamingo, 50 },
                    { RoleTypeId.AlphaFlamingo, 50 }
                }
            }
        };

        public List<Loadout> ZombieFlamingoLoadouts { get; set; } = new List<Loadout>()
        {
            new Loadout()
            {
                Roles = new Dictionary<RoleTypeId, int>() { { RoleTypeId.ZombieFlamingo, 100 } }
            }
        };
        */
        public List<string> ZombieScreams { get; set; } = new List<string>()
        {
            "human_death_01.ogg",
            "human_death_02.ogg"
        };
    }
}