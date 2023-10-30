using System;
using AutoEvent.Events.EventArgs;
using MEC;
using PlayerRoles;
using PlayerStatsSystem;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using System.Linq;
using AutoEvent.API;
using AutoEvent.API.Enums;
using CustomPlayerEffects;
using InventorySystem.Items.MarshmallowMan;
using PluginAPI.Core;
using UnityEngine;

namespace AutoEvent.Games.HideAndSeek
{
    public class EventHandler
    {
        private HideAndSeek.Plugin _plugin { get; set; }
        public EventHandler(HideAndSeek.Plugin ev)
        {
            _plugin = ev as HideAndSeek.Plugin;
        }    
    public void OnPlayerDamage(PlayerDamageArgs ev)
        {
            if (ev.DamageType == DeathTranslations.Falldown.Id)
            {
                ev.IsAllowed = false;
            }

            if (ev.Target.EffectsManager.GetEffect<SpawnProtected>().IsEnabled)
            {
                ev.IsAllowed = false;
                return;
            }
            if (ev.Attacker != null)
            {
                ev.IsAllowed = false;
                bool isAttackerTagger = ev.Attacker.Items.Any(r => r.ItemTypeId == _plugin.Config.TaggerWeapon) || ev.Attacker.EffectsManager.GetEffect<MarshmallowEffect>();
                bool isTargetTagger = ev.Target.Items.Any(r => r.ItemTypeId == _plugin.Config.TaggerWeapon) || ev.Attacker.EffectsManager.GetEffect<MarshmallowEffect>();
                if (!isAttackerTagger || isTargetTagger)
                {
                    ev.IsAllowed = false;
                    return;
                }

                bool isLastPlayers = Player.GetPlayers().Count(ply => ply.HasLoadout(_plugin.Config.PlayerLoadouts)) <= _plugin.Config.PlayersRequiredForBreachScannerEffect;
                /*switch (ev.DamageTypze)
                {
                    case (byte)DamageType.GrenadeExplosion:
                        if (_plugin.Config.TaggerWeapon == ItemType.GrenadeHE)
                            ev.IsAllowed = true;
                        return; 
                    case (byte)DamageType.Jailbird:
                        if (_plugin.Config.TaggerWeapon == ItemType.Jailbird)
                            isAllowed = true;
                        break;
                    case (byte)DamageType.Scp018:
                        if (_plugin.Config.TaggerWeapon == ItemType.SCP018)
                            isAllowed = true;
                        break;
                    default:
                        if (!(_plugin.Config.Range > 0 && Vector3.Distance(ev.Attacker.Position, ev.Target.Position) <=
                                _plugin.Config.Range))
                            isAllowed = true;
                        else
                        {
                            ev.IsAllowed = false;
                            return;
                        }
                        break;
                }*/

                    DebugLogger.LogDebug($"{ev.Target.Nickname} has been tagged by {ev.Attacker.Nickname}");
                    ev.IsAllowed = false;
                    ev.Attacker.EffectsManager.EnableEffect<SpawnProtected>(_plugin.Config.NoTagBackDuration, false);
                    ev.Attacker.GiveLoadout(_plugin.Config.PlayerLoadouts, LoadoutFlags.IgnoreItems | LoadoutFlags.IgnoreWeapons | LoadoutFlags.IgnoreGodMode);
                    ev.Attacker.ClearInventory();
                    for (int i = 0; i < 8; i++)
                    {
                        try
                        {
                            ev.Attacker.AddItem(ItemType.Coin);
                        }
                        catch (Exception)
                        {
                            break;
                        }
                    }
                    if (isLastPlayers)
                        ev.Attacker.GiveEffect(StatusEffect.Scanned, 255, 0f, false);
                    
                    
                    ev.Target.GiveLoadout(_plugin.Config.TaggerLoadouts, LoadoutFlags.IgnoreItems | LoadoutFlags.IgnoreWeapons | LoadoutFlags.IgnoreGodMode);
                    ev.Target.ClearInventory();
                    if (_plugin.Config.HalloweenMelee)
                    {
                        ev.Target.EffectsManager.EnableEffect<MarshmallowEffect>();
                    }
                    else
                    {
                        
                    var weapon = ev.Target.AddItem(_plugin.Config.TaggerWeapon);
                    if(weapon.ItemTypeId is ItemType.GrenadeHE)
                        weapon.ExplodeOnCollision(true);
                    if(weapon.ItemTypeId is ItemType.SCP018)
                        weapon.MakeRock(new RockSettings(false, 1f, false, false, true));
                    if(isLastPlayers)
                        ev.Target.GiveEffect(StatusEffect.Scanned, 0, 1f, false);
                
                    Timing.CallDelayed(0.1f, () =>
                    {
                        ev.Target.CurrentItem = weapon;
                    });
                    }
                    return;

            }
        }
    
    [PluginEvent(ServerEventType.GrenadeExploded)]
    public void OnGrenadeExplode(GrenadeExplodedEvent ev)
    {
        if (_plugin.Config.TaggerWeapon != ItemType.GrenadeHE)
            return;
        bool noRange = _plugin.Config.Range == 0;
        // Player.GetPlayers().Where(x => x.IsAlive && x.)
    }
    

        [PluginEvent(ServerEventType.PlayerJoined)]
        public void OnJoin(PlayerJoinedEvent ev)
        {
            ev.Player.SetRole(RoleTypeId.Spectator);
        }

        public void OnTeamRespawn(TeamRespawnArgs ev) => ev.IsAllowed = false;
        public void OnSpawnRagdoll(SpawnRagdollArgs ev) => ev.IsAllowed = false;
        public void OnPlaceBullet(PlaceBulletArgs ev) => ev.IsAllowed = false;
        public void OnPlaceBlood(PlaceBloodArgs ev) => ev.IsAllowed = false;
        public void OnDropItem(DropItemArgs ev) => ev.IsAllowed = false;
        public void OnDropAmmo(DropAmmoArgs ev) => ev.IsAllowed = false;
    }
}
