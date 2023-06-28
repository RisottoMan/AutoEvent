using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using PlayerRoles;
using UnityEngine;

namespace AutoEvent.Events.Glass
{
    public class EventHandler
    {
        public void OnJoin(VerifiedEventArgs ev)
        {
            ev.Player.Role.Set(RoleTypeId.Spectator);
        }
        public void OnDroppingItem(DroppingItemEventArgs ev)
        {
            ev.IsAllowed = false;
        }
        public void OnTeamRespawn(RespawningTeamEventArgs ev)
        {
            ev.IsAllowed = false;
        }
        public void OnPressVoice(VoiceChattingEventArgs ev)
        {
            foreach (Player player in Player.List)
            {
                if (ev.Player != player)
                {
                    ev.Player.GameObject.TryGetComponent<Rigidbody>(out Rigidbody rig);
                    rig.AddForce(player.Transform.forward * 0.5f, ForceMode.Impulse);
                }
            }
        }
    }
}
