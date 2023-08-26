using AutoEvent.API.Schematic.Objects;
using System.Linq;
using UnityEngine;

namespace AutoEvent.Games.Knives
{
    internal class RandomClass
    {
        public static Vector3 GetSpawnPosition(SchematicObject GameMap, bool isScientist)
        {
            string spawnName = isScientist ? "Spawnpoint" : "Spawnpoint1";
            return GameMap.AttachedBlocks.Where(x => x.name == spawnName).ToList().RandomItem().transform.position;
        }
    }
}
