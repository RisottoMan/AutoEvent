using PlayerRoles;
using System.Collections.Generic;

namespace AutoEvent.Events.DeathParty.Features
{
    public class RandomRoles
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
    }
}