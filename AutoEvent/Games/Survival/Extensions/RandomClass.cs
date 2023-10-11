using AutoEvent.API.Schematic.Objects;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AutoEvent.Games.Survival
{
    public class RandomClass
    {
        public static Vector3 GetSpawnPosition(SchematicObject GameMap)
        {
            return GameMap.AttachedBlocks.Where(x => x.name == "Spawnpoint").ToList().RandomItem().transform.position;
        }
    }
}
