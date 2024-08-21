using System.Collections.Generic;
using AutoEvent.Events.EventArgs;
using InventorySystem.Configs;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;

namespace AutoEvent.Games.Deathrun;
public class EventHandler
{
    [PluginEvent(ServerEventType.PlayerSpawn)]
    public void OnSpawning(PlayerSpawnEvent ev)
    {
        foreach (KeyValuePair<ItemType, ushort> AmmoLimit in InventoryLimits.StandardAmmoLimits)
        {
            ev.Player.SetAmmo(AmmoLimit.Key, AmmoLimit.Value);
        }
    }

    [PluginEvent(ServerEventType.PlayerDying)]
    public void OnPlayerDying(PlayerDyingEvent ev)
    {
        DebugLogger.LogDebug("Play music");
        Extensions.PlayPlayerAudio(ev.Player, "Death-Sound.ogg", 7);
    }
    
    public void OnTeamRespawn(TeamRespawnArgs ev) => ev.IsAllowed = false;
    public void OnDropItem(DropItemArgs ev) => ev.IsAllowed = false;
    public void OnDropAmmo(DropAmmoArgs ev) => ev.IsAllowed = false;
}