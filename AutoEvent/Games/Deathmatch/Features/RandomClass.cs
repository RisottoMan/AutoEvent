using System.Linq;
using AutoEvent.API;
using UnityEngine;

using MapEditorReborn.API.Features.Objects;

namespace AutoEvent.Games.Deathmatch
{
    internal class RandomClass
    {
        public static Vector3 GetRandomPosition(MapObject GameMap)
        {
            if (GameMap is null)
            {
                DebugLogger.LogDebug("Map is null");
                return Vector3.zero;
            }

            if (GameMap.AttachedBlocks is null)
            {
                DebugLogger.LogDebug("Attached Blocks is null");
                return Vector3.zero;
            }

            var spawnpoint = GameMap.AttachedBlocks.Where(x => x.name == "Spawnpoint").ToList().RandomItem();
            if (spawnpoint is null)
            {
                DebugLogger.LogDebug("Spawnpoint is null");
                return Vector3.zero;
            }

            return spawnpoint.transform.position;
        }
    }
}
