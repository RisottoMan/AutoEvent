using System.Linq;
using MER.Lite.Objects;
using UnityEngine;

namespace AutoEvent.Games.Spleef
{
    internal class RandomClass
    {
        public static Vector3 GetSpawnPosition(SchematicObject GameMap)
        {
            return GameMap.AttachedBlocks.Where(x => x.name == "Spawnpoint").FirstOrDefault().transform.position;
        }
    }
}
