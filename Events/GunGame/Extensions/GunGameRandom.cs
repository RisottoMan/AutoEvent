using MapEditorReborn.API.Features.Objects;
using PlayerRoles;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AutoEvent.Events.GunGame
{
    internal class GunGameRandom
    {
        private static List<RoleTypeId> HumanRoles = new List<RoleTypeId>()
        {
            RoleTypeId.ClassD,
            RoleTypeId.Scientist,
            RoleTypeId.NtfSergeant,
            RoleTypeId.ChaosRifleman,
            RoleTypeId.FacilityGuard
        };
        public static RoleTypeId GetRandomRole() => HumanRoles.RandomItem();
        public static Vector3 GetRandomPosition(SchematicObject GameMap)
        {
            return GameMap.AttachedBlocks.Where(x => x.name == "Spawnpoint").ToList().RandomItem().transform.position;
        }
    }
}
