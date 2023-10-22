using MER.Lite.Objects;
using System.Linq;
using UnityEngine;

namespace AutoEvent.Games.Puzzle
{
    internal class RandomClass
    {
        public static Vector3 GetSpawnPosition(SchematicObject GameMap)
        {
            return GameMap.AttachedBlocks.Where(x => x.name == "Spawnpoint").FirstOrDefault().transform.position;
        }
    }
}
