using AutoEvent.API.Schematic.Objects;
using System.Linq;
using UnityEngine;

namespace AutoEvent.Games.Jail
{
    public class JailRandom
    {
        public static Vector3 GetRandomPosition(SchematicObject GameMap, bool isMtf)
        {
            string spawnName = isMtf ? "SpawnpointMtf" : "Spawnpoint";
            return GameMap.AttachedBlocks.Where(x => x.name == spawnName).ToList().RandomItem().transform.position;
        }
    }
}
