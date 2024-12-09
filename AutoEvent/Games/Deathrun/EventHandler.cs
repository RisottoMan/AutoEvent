using AutoEvent.Events.EventArgs;
using PlayerRoles;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;

namespace AutoEvent.Games.Deathrun;
public class EventHandler
{
    [PluginEvent(ServerEventType.PlayerJoined)]
    public void OnJoin(PlayerJoinedEvent ev)
    {
        ev.Player.SetRole(RoleTypeId.Spectator);
    }
    
    public void OnTeamRespawn(TeamRespawnArgs ev) => ev.IsAllowed = false;
    public void OnDropItem(DropItemArgs ev) => ev.IsAllowed = false;
    public void OnDropAmmo(DropAmmoArgs ev) => ev.IsAllowed = false;
}