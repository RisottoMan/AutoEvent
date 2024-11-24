using AutoEvent.Events.EventArgs;
using PlayerRoles;
using PlayerStatsSystem;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using System.Linq;

namespace AutoEvent.Games.MusicalChairs;
public class EventHandler
{
    Plugin _plugin;
    public EventHandler(Plugin plugin)
    {
        _plugin = plugin;
    }

    public void OnDamage(PlayerDamageArgs ev)
    {
        // The players will not die from the explosion
        if (ev.AttackerHandler is ExplosionDamageHandler damageHandler)
        {
            damageHandler.Damage = 0;
        }
    }

    public void OnUsingStamina(UsingStaminaArgs ev)
    {
        ev.IsAllowed = false;
    }

    [PluginEvent(ServerEventType.PlayerDeath)]
    public void OnPlayerDeath(PlayerDeathEvent ev)
    {
        // Remove the dead player from the dictionary
        if (_plugin.PlayerDict.ContainsKey(ev.Player))
        {
            _plugin.PlayerDict.Remove(ev.Player);
        }
        
        // If the player is dead, then remove the last platform
        int playerCount = Player.GetPlayers().Count(r => r.IsAlive);
        if (playerCount > 0)
        {
            _plugin.Platforms = Functions.RearrangePlatforms(playerCount, _plugin.Platforms, _plugin.MapInfo.Position);
        }
    }
    
    [PluginEvent(ServerEventType.PlayerLeft)]
    public void OnPlayerLeft(PlayerLeftEvent ev)
    {
        // Remove the left player from the dictionary
        if (_plugin.PlayerDict.ContainsKey(ev.Player))
        {
            _plugin.PlayerDict.Remove(ev.Player);
        }
    }

    public void OnTeamRespawn(TeamRespawnArgs ev) => ev.IsAllowed = false;
    public void OnSpawnRagdoll(SpawnRagdollArgs ev) => ev.IsAllowed = false;
    public void OnPlaceBullet(PlaceBulletArgs ev) => ev.IsAllowed = false;
    public void OnPlaceBlood(PlaceBloodArgs ev) => ev.IsAllowed = false;
    public void OnDropItem(DropItemArgs ev) => ev.IsAllowed = false;
    public void OnDropAmmo(DropAmmoArgs ev) => ev.IsAllowed = false;
}
