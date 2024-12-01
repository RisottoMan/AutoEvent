using AutoEvent.Events.EventArgs;
using PlayerRoles;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using System;
using UnityEngine;

namespace AutoEvent.Games.CounterStrike
{
    public class EventHandler
    {
        Plugin _plugin;
        public EventHandler(Plugin plugin)
        {
            _plugin = plugin;
        }

        public void OnSearchPickUpItem(SearchPickUpItemArgs ev)
        {
            if (ev.Pickup.Info.ItemId != ItemType.SCP018)
                return;

            if (_plugin.BombState == BombState.NoPlanted)
            {
                if (ev.Player.IsChaos)
                {
                    _plugin.Winner = ev.Player;
                    _plugin.BombState = BombState.Planted;
                    _plugin.RoundTime = new TimeSpan(0, 0, 35);

                    _plugin.BombObject.transform.position = ev.Pickup.Position;
                    _plugin.Buttons.ForEach(r => GameObject.Destroy(r));

                    Extensions.PlayAudio("BombPlanted.ogg", 5, false);
                    ev.Player.ReceiveHint(_plugin.Translation.YouPlanted, 3);
                }
            }
            else if (_plugin.BombState == BombState.Planted)
            {
                if (ev.Player.IsNTF && Vector3.Distance(ev.Player.Position, _plugin.BombObject.transform.position) < 3)
                {
                    _plugin.Winner = ev.Player;
                    _plugin.BombState = BombState.Defused;
                    ev.Player.ReceiveHint(_plugin.Translation.YouDefused, 3);
                }
            }

            ev.IsAllowed = false;
        }

        [PluginEvent(ServerEventType.PlayerJoined)]
        public void OnJoin(PlayerJoinedEvent ev)
        {
            Extensions.SetRole(ev.Player, RoleTypeId.Spectator, RoleSpawnFlags.None);
        }

        public void OnTeamRespawn(TeamRespawnArgs ev) => ev.IsAllowed = false;
        public void OnSpawnRagdoll(SpawnRagdollArgs ev) => ev.IsAllowed = false;
        public void OnPlaceBullet(PlaceBulletArgs ev) => ev.IsAllowed = false;
        public void OnPlaceBlood(PlaceBloodArgs ev) => ev.IsAllowed = false;
        public void OnDropAmmo(DropAmmoArgs ev) => ev.IsAllowed = false;
    }
}
