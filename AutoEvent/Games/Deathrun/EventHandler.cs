using System.Collections.Generic;
using AutoEvent.Events.EventArgs;
using InventorySystem.Configs;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using Utils.Networking;

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
    
    public void OnTeamRespawn(TeamRespawnArgs ev) => ev.IsAllowed = false;
    public void OnDropItem(DropItemArgs ev) => ev.IsAllowed = false;
    public void OnDropAmmo(DropAmmoArgs ev) => ev.IsAllowed = false;
}
