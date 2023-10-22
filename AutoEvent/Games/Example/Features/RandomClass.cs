using System.Collections.Generic;
using MER.Lite.Objects;
using PluginAPI.Core;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AutoEvent.Games.Example
{
    internal static class RandomClass
    {
        public static Vector3 GetSpawnPosition(SchematicObject GameMap, bool isMtf)
        {
            string spawnName = isMtf ? "Spawnpoint" : "Spawnpoint1";
            return GameMap.AttachedBlocks.Where(x => x.name == spawnName).FirstOrDefault().transform.position;
        }
    }
}
