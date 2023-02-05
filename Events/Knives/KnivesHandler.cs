using PlayerRoles;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using InventorySystem;
using HarmonyLib;
using System;
using Exiled.API.Features;

namespace AutoEvent.Events
{
    internal class KnivesHandler
    {
        public static void OnJoin(VerifiedEventArgs ev)
        {
            ev.Player.Role.Set(RoleTypeId.Spectator);
        }
        public static void OnDropItem(DroppingItemEventArgs ev)
        {
            ev.IsAllowed = false;
        }
        public static void OnTeamRespawn(RespawningTeamEventArgs ev)
        {
            ev.IsAllowed = false;
        }
    }
}
