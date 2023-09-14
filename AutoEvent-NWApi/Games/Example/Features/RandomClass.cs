using System.Collections.Generic;
using AutoEvent.API.Schematic.Objects;
using PluginAPI.Core;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AutoEvent.Games.Example
{
    internal static class RandomClass
    {
        public static void CreateSoldier(List<Loadout> loadouts, Player player)
        {
            int totalChance = loadouts.Sum(x => x.Chance);
            Loadout loadout;
            if (loadouts.Count == 1)
            {
                loadout = loadouts[0];
                goto assignLoadout;
            }
            
            for (int i = 0; i < loadouts.Count - 1; i++)
            {
                if (Random.Range(0, totalChance) <= loadouts[i].Chance)
                {
                    loadout = loadouts[i];
                    goto assignLoadout;
                }
            }
            loadout = loadouts[loadouts.Count - 1];
            
            assignLoadout:
            foreach (var item in loadout.Items)
            {
                player.AddItem(item);
            }
            player.Health = loadout.Health;
            player.SetPlayerScale(loadout.Size);
        }

        public static Vector3 GetSpawnPosition(SchematicObject GameMap, bool isMtf)
        {
            string spawnName = isMtf ? "Spawnpoint" : "Spawnpoint1";
            return GameMap.AttachedBlocks.Where(x => x.name == spawnName).FirstOrDefault().transform.position;
        }
    }
}
