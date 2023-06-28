using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using MEC;
using PlayerRoles;
using UnityEngine;

namespace AutoEvent.Events.Football
{
    public class EventHandler
    {
        public void OnJoin(VerifiedEventArgs ev)
        {
            if (Random.Range(0, 2) == 0)
            {
                ev.Player.Role.Set(RoleTypeId.NtfCaptain, RoleSpawnFlags.None);
            }
            else
            {
                ev.Player.Role.Set(RoleTypeId.ClassD, RoleSpawnFlags.None);
            }
            //ev.Player.Position = GameMap.GameObject.transform.position + new Vector3(0, 5, 0);
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
