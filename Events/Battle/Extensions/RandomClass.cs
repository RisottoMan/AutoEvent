using Exiled.API.Features;
using MapEditorReborn.API.Features.Objects;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AutoEvent.Events.Battle
{
    internal class RandomClass
    {
        public static void CreateSoldier(Player player)
        {
            switch (Random.Range(0, 3))
            {
                case 0:
                    {
                        //player.Broadcast("Вы играете за ПЕХОТУ", 6);  // Может сделать в масс-бродкасте описание класса игрока - Вы медик - лечите игроков...
                        player.AddItem(ItemType.GunE11SR);
                        player.AddItem(ItemType.Medkit, 2);
                        player.AddItem(ItemType.GrenadeHE, 2);
                        player.AddItem(ItemType.ArmorCombat);
                        player.AddItem(ItemType.SCP1853);
                        player.AddItem(ItemType.Adrenaline);
                    }
                    break;
                case 1:
                    {
                        //player.Broadcast("Вы играете за МЕДИКА", 6);
                        player.AddItem(ItemType.GunShotgun);
                        player.AddItem(ItemType.Medkit, 5);
                        player.AddItem(ItemType.ArmorCombat);
                        player.AddItem(ItemType.SCP500);
                    }
                    break;
                case 2:
                    {
                        //player.Broadcast("Вы играете за ТАНК", 6);
                        player.AddAhp(100, 500);
                        player.AddItem(ItemType.GunLogicer);
                        player.AddItem(ItemType.ArmorHeavy);
                        player.AddItem(ItemType.GrenadeHE, 2);
                        player.AddItem(ItemType.SCP500, 2);
                        player.AddItem(ItemType.SCP1853);
                        player.AddItem(ItemType.Medkit);
                    }
                    break;
            }
        }
        public static Vector3 GetSpawnPosition(SchematicObject GameMap)
        {
            return GameMap.AttachedBlocks.Where(x => x.name == "Spawnpoint").FirstOrDefault().transform.position;
        }
    }
}
