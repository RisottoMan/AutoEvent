using AutoEvent.API.Schematic.Objects;
using System.Linq;
using UnityEngine;

namespace AutoEvent.Games.Football.Features
{
    internal class RandomClass
    {
        public static Vector3 GetSpawnPosition(SchematicObject GameMap, bool isMtf)
        {
            string spawnName = isMtf ? "Spawnpoint" : "Spawnpoint1";
            return GameMap.AttachedBlocks.Where(x => x.name == spawnName).FirstOrDefault().transform.position;
        }
    }
}
