using MapEditorReborn.API.Features.Objects;
using System.Linq;
using UnityEngine;

namespace AutoEvent.Events.Deathmatch
{
    internal class RandomClass
    {
        public static Vector3 GetRandomPosition(SchematicObject GameMap)
        {
            return GameMap.AttachedBlocks.Where(x => x.name == "Spawnpoint").ToList().RandomItem().transform.position;
        }
    }
}
