using AutoEvent.Events.EventArgs;
using CustomPlayerEffects;
using InventorySystem.Configs;
using MEC;
using PlayerRoles;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using System.Collections.Generic;
using System.Linq;
using Player = PluginAPI.Core.Player;

namespace AutoEvent.Games.Deathmatch
{
    public class EventHandler
    {
        Plugin _plugin;
        public EventHandler(Plugin plugin)
        {
            _plugin = plugin;
        }

        [PluginEvent(ServerEventType.PlayerJoined)]
        public void OnJoin(PlayerJoinedEvent ev)
        {
            if (Player.GetPlayers().Count(r => r.Team == Team.FoundationForces) > Player.GetPlayers().Count(r => r.Team == Team.ChaosInsurgency))
            {
                Extensions.SetRole(ev.Player, RoleTypeId.ChaosRifleman, RoleSpawnFlags.None);
            }
            else
            {
                Extensions.SetRole(ev.Player, RoleTypeId.NtfSergeant, RoleSpawnFlags.None);
            }

            var item = ev.Player.AddItem(RandomClass.RandomItems.RandomItem());
            ev.Player.AddItem(ItemType.ArmorCombat);

            ev.Player.EffectsManager.EnableEffect<Scp1853>(150);
            ev.Player.EffectsManager.ChangeState<Scp1853>(255);
            ev.Player.EffectsManager.EnableEffect<MovementBoost>(150);
            ev.Player.EffectsManager.ChangeState<MovementBoost>(10);

            ev.Player.Position = RandomClass.GetRandomPosition(_plugin.MapInfo.Map);

            Timing.CallDelayed(0.1f, () =>
            {
                ev.Player.CurrentItem = item;
            });
        }

        [PluginEvent(ServerEventType.PlayerReloadWeapon)]
        public void OnReloading(PlayerReloadWeaponEvent ev)
        {
            SetMaxAmmo(ev.Player);
        }

        [PluginEvent(ServerEventType.PlayerSpawn)]
        public void OnSpawning(PlayerSpawnEvent ev)
        {
            SetMaxAmmo(ev.Player);
        }

        private void SetMaxAmmo(Player pl)
        {
            foreach (KeyValuePair<ItemType, ushort> AmmoLimit in InventoryLimits.StandardAmmoLimits)
            {
                pl.SetAmmo(AmmoLimit.Key, AmmoLimit.Value);
            }
        }

        public void OnPlayerDying(PlayerDyingArgs ev)
        {
            ev.IsAllowed = false;

            if (ev.Target.Team == Team.FoundationForces)
            {
                _plugin.ChaosKills++;
            }
            else if (ev.Target.Team == Team.ChaosInsurgency)
            {
                _plugin.MtfKills++;
            }

            ev.Target.EffectsManager.EnableEffect<Flashed>(0.1f);
            ev.Target.Position = RandomClass.GetRandomPosition(_plugin.MapInfo.Map);
            ev.Target.Health = 100;

            Timing.CallDelayed(0.1f, () =>
            {
                ev.Target.CurrentItem = ev.Target.Items.ElementAt(0);
            });
        }

        public void OnHandCuff(HandCuffArgs ev) => ev.IsAllowed = false;
        public void OnTeamRespawn(TeamRespawnArgs ev) => ev.IsAllowed = false;
        public void OnSpawnRagdoll(SpawnRagdollArgs ev) => ev.IsAllowed = false;
        public void OnPlaceBullet(PlaceBulletArgs ev) => ev.IsAllowed = false;
        public void OnPlaceBlood(PlaceBloodArgs ev) => ev.IsAllowed = false;
        public void OnDropItem(DropItemArgs ev) => ev.IsAllowed = false;
        public void OnDropAmmo(DropAmmoArgs ev) => ev.IsAllowed = false;
    }
}
