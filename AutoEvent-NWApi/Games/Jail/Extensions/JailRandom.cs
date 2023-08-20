using AutoEvent.API.Schematic.Objects;
using System.Linq;
using UnityEngine;

namespace AutoEvent.Games.Jail
{
    public class JailRandom
    {
        public static Vector3 GetRandomPosition(SchematicObject GameMap, bool isMtf)
        {
            if (isMtf) return GameMap.AttachedBlocks.First(x => x.name == "SpawnpointMtf").transform.position;
            else return GameMap.AttachedBlocks.Where(x => x.name == "Spawnpoint").ToList().RandomItem().transform.position;
        }
    }
}
