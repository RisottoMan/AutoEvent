using AutoEvent.API.Schematic.Objects;
using System.Linq;
using UnityEngine;

namespace AutoEvent.Games.Versus.Features
{
    internal class RandomClass
    {
        public static Vector3 GetSpawnPosition(SchematicObject GameMap, bool isScientist)
        {
            var spawnpoints = GameMap.AttachedBlocks.Where(x => x.name == "Spawnpoint");

            if (isScientist) return spawnpoints.ElementAt(0).transform.position;
            else return spawnpoints.ElementAt(1).transform.position;
        }
    }
}
