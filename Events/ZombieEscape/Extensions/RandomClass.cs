using MapEditorReborn.API.Features.Objects;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AutoEvent.Events.ZombieEscape
{
    public class RandomClass
    {
        public static List<ItemType> RandomItems { get; set; } = new()
        {
            ItemType.GunAK,
            ItemType.GunE11SR,
            ItemType.GunShotgun
        };

        public static ItemType GetRandomGun()
        {
            return RandomItems.RandomItem();
        }

        public static Vector3 GetSpawnPosition(SchematicObject GameMap)
        {
            return GameMap.AttachedBlocks.Where(x => x.name == "Spawnpoint").ToList().RandomItem().transform.position;
        }
    }
}
