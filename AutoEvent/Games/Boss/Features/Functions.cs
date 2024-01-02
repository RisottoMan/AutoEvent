using MER.Lite;
using MER.Lite.Objects;
using PluginAPI.Core;
using System.Collections.Generic;
using UnityEngine;

namespace AutoEvent.Games.Boss.Features
{
    public class Functions
    {
        public static List<SchematicObject> SantaSchematic { get; set; }
        public static void CreateSchematicBoss(Player player)
        {
            if (SantaSchematic == null)
            {
                SantaSchematic = new List<SchematicObject>();
            }

            var schematic = ObjectSpawner.SpawnSchematic(
                "SantaClaus", 
                player.Position + new Vector3(
                    0,
                    -2.244f,
                    0
                    ),
                Quaternion.Euler(player.Rotation),
                new Vector3(
                    1f, 
                    1f, 
                    1f
                    ));

            schematic.transform.parent = player.ReferenceHub.transform;
            SantaSchematic.Add(schematic);
        }

        public static void RemoveSchematicBosses()
        {
            if (SantaSchematic != null)
            {
                foreach (var schematic in SantaSchematic)
                {
                    schematic.Destroy();
                }
            }
        }
    }
}
