using AutoEvent.API.Schematic.Objects;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AutoEvent.Games.Deathmatch
{
    internal class RandomClass
    {
        public static List<ItemType> RandomItems { get; set; } = new()
        {
            ItemType.GunAK,
            ItemType.GunE11SR,
            ItemType.GunShotgun
        };
        public static Vector3 GetRandomPosition(SchematicObject GameMap)
        {
            return GameMap.AttachedBlocks.Where(x => x.name == "Spawnpoint").ToList().RandomItem().transform.position;
        }
    }
}
