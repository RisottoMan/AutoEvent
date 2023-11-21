using MER.Lite.Objects;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AutoEvent.Games.AllDeathmatch
{
    internal class RandomClass
    {
        public static List<GameObject> GetAllSpawnpoint(SchematicObject GameMap)
        {
            if (GameMap is null)
            {
                DebugLogger.LogDebug("Map is null");
            }

            if (GameMap.AttachedBlocks is null)
            {
                DebugLogger.LogDebug("Attached Blocks is null");
            }

            List<GameObject> spawnpoints = GameMap.AttachedBlocks.Where(x => x.name == "Spawnpoint_Deathmatch").ToList();
            return spawnpoints;
        }
    }
}
