using System.Collections.Generic;
using System.ComponentModel;
using AutoEvent.API;
using AutoEvent.Interfaces;

namespace AutoEvent.Games.Battle
{
    public class BattleConfig : EventConfig
    {

        [Description(
            "A list that contains maps. You also need to add maps to the Schematic folder. Specify the folder name of map.")]
        public List<string> ListOfMap { get; set; } = new List<string>()
        {
            "Battle",
        };

        [Description(
            "A dictionary containing the name of the music and volume. They need to be manually added to the Music folder.")]
        public Dictionary<string, byte> ListOfMusic { get; set; } = new Dictionary<string, byte>()
        {
            ["MetalGearSolid.ogg"] = 10,
        };

        [Description("A List of Loadouts to use.")]
        public List<Loadout> Loadouts { get; set; } = new List<Loadout>()
        {
            new Loadout()
            {
                Health = 100,
                Chance = 33,
                Items = new List<ItemType>()
                {
                    ItemType.GunE11SR, ItemType.Medkit, ItemType.Medkit,
                    ItemType.ArmorCombat, ItemType.SCP1853, ItemType.Adrenaline,
                }
            },
            new Loadout()
            {
                Health = 115,
                Chance = 33,
                Items = new List<ItemType>()
                {
                    ItemType.GunShotgun, ItemType.Medkit, ItemType.Medkit,
                    ItemType.Medkit, ItemType.Medkit, ItemType.Medkit,
                    ItemType.ArmorCombat, ItemType.SCP500,
                }
            },
            new Loadout()
            {
                Health = 200,
                Chance = 33,
                Items = new List<ItemType>()
                {
                    ItemType.GunLogicer, ItemType.ArmorHeavy, ItemType.SCP500,
                    ItemType.SCP500, ItemType.SCP1853, ItemType.Medkit,
                }
            }
        };
    }
}