using MapEditorReborn.API.Features.Objects;
using System.Linq;
using UnityEngine;

namespace AutoEvent.Events.HideAndSeek.Features
{
    internal class RandomClass
    {
        public static Vector3 GetSpawnPosition(SchematicObject GameMap)
        {
            return GameMap.AttachedBlocks.Where(x => x.name == "Spawnpoint").FirstOrDefault().transform.position;
        }
    }
}
