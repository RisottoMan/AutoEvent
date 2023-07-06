using MapEditorReborn.API.Features.Objects;
using System.Linq;
using UnityEngine;

namespace AutoEvent.Events.Jail
{
    public class JailRandom
    {
        public static Vector3 GetRandomPosition(SchematicObject GameMap, bool isMtf)
        {
            if (isMtf) return GameMap.AttachedBlocks.Where(x => x.name == "SpawnpointMtf").ToList().RandomItem().transform.position;
            else return GameMap.AttachedBlocks.First(x => x.name == "Spawnpoint").transform.position;
        }
    }
}
