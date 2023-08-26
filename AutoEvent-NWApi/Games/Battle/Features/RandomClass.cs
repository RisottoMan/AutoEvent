using AutoEvent.API.Schematic.Objects;
using PluginAPI.Core;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AutoEvent.Games.Battle
{
    internal class RandomClass
    {
        public static void CreateSoldier(Player player)
        {
            switch (Random.Range(0, 3))
            {
                case 0:
                    {
                        player.AddItem(ItemType.GunE11SR);
                        player.AddItem(ItemType.Medkit);
                        player.AddItem(ItemType.Medkit);
                        player.AddItem(ItemType.ArmorCombat);
                        player.AddItem(ItemType.SCP1853);
                        player.AddItem(ItemType.Adrenaline);
                    }
                    break;
                case 1:
                    {
                        player.AddItem(ItemType.GunShotgun);
                        player.AddItem(ItemType.Medkit);
                        player.AddItem(ItemType.Medkit);
                        player.AddItem(ItemType.Medkit);
                        player.AddItem(ItemType.Medkit);
                        player.AddItem(ItemType.Medkit);
                        player.AddItem(ItemType.ArmorCombat);
                        player.AddItem(ItemType.SCP500);
                    }
                    break;
                case 2:
                    {
                        player.ArtificialHealth = 100;
                        player.AddItem(ItemType.GunLogicer);
                        player.AddItem(ItemType.ArmorHeavy);
                        player.AddItem(ItemType.SCP500);
                        player.AddItem(ItemType.SCP500);
                        player.AddItem(ItemType.SCP1853);
                        player.AddItem(ItemType.Medkit);
                    }
                    break;
            }
        }

        public static Vector3 GetSpawnPosition(SchematicObject GameMap, bool isMtf)
        {
            if (isMtf) return GameMap.AttachedBlocks.Where(x => x.name == "Spawnpoint").FirstOrDefault().transform.position;
            else return GameMap.AttachedBlocks.Where(x => x.name == "Spawnpoint1").FirstOrDefault().transform.position;
        }
    }
}
