using PlayerRoles;
using System.Collections.Generic;

namespace AutoEvent.Events.DeathParty
{
    internal class RandomRoles
    {
        public static RoleTypeId GetRandomRole() => HumanRoles.RandomItem();

        private static List<RoleTypeId> HumanRoles = new List<RoleTypeId>()
        {
            RoleTypeId.ClassD,
            RoleTypeId.Scientist,
            RoleTypeId.NtfSergeant,
            RoleTypeId.ChaosRifleman,
            RoleTypeId.FacilityGuard
        };
    }
}