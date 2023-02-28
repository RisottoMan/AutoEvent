using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AutoEvent.Events
{
    internal class DeathmatchClass
    {
        public static RoleTypeId GetRandomClass(Team team)
        {
            if (team == Team.FoundationForces)
            {
                switch(Random.Range(1, 4))
                {
                    case 1: return RoleTypeId.NtfCaptain;
                    case 2: return RoleTypeId.NtfPrivate;
                    case 3: return RoleTypeId.NtfSergeant;
                    case 4: return RoleTypeId.NtfSpecialist;
                }
            }
            if (team == Team.ChaosInsurgency)
            {
                switch (Random.Range(1, 4))
                {
                    case 1: return RoleTypeId.ChaosRepressor;
                    case 2: return RoleTypeId.ChaosRifleman;
                    case 3: return RoleTypeId.ChaosConscript;
                    case 4: return RoleTypeId.ChaosMarauder;
                }
            }
            return RoleTypeId.NtfCaptain;
        }
    }
}
