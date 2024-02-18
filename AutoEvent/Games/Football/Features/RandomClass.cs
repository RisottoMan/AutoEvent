using MER.Lite.Objects;
using System.Linq;
using UnityEngine;

namespace AutoEvent.Games.Football;
internal class RandomClass
{
    public static Vector3 GetSpawnPosition(SchematicObject GameMap, bool isMtf)
    {
        int value = isMtf ? 0 : 1;
        return GameMap.AttachedBlocks.Where(x => x.name == "Spawnpoint").ElementAt(value).transform.position;
    }
}
