using System;
using System.Collections.Generic;
using AutoEvent.API.Schematic.Objects;
using PluginAPI.Core;
using System.Linq;
using AutoEvent.Games.Example;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AutoEvent.Games.Battle
{
    internal class RandomClass
    {
        public static Vector3 GetSpawnPosition(SchematicObject GameMap, bool isMtf)
        {
            string spawnName = isMtf ? "Spawnpoint" : "Spawnpoint1";
            return GameMap.AttachedBlocks.Where(x => x.name == spawnName).FirstOrDefault().transform.position;
        }
    }
}
