using MER.Lite.Objects;
using System.Linq;
using UnityEngine;

namespace AutoEvent.Games.BuckshotRoulette
{
    internal class RandomClass
    {
        public static Vector3 GetSpawnPosition(SchematicObject GameMap, bool isScientist)
        {
            int value = isScientist ? 0 : 1;
            return GameMap.AttachedBlocks.Where(x => x.name == "Spawnpoint").ElementAt(value).transform.position;
        }
    }
}
