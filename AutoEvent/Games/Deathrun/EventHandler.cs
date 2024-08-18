using AutoEvent.Events.EventArgs;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using Utils.Networking;

namespace AutoEvent.Games.Deathrun;
public class EventHandler
{
    [PluginEvent(ServerEventType.PlayerDeath)]
    public void OnDeath(PlayerDeathEvent ev)
    {
        //new PlayerRoles.PlayableScps.HumeShield.DynamicHumeShieldController.ShieldBreakMessage()
        //    { Target = ev.Player.ReferenceHub }.SendToAuthenticated();
    }
    public void OnTeamRespawn(TeamRespawnArgs ev) => ev.IsAllowed = false;
    public void OnDropItem(DropItemArgs ev) => ev.IsAllowed = false;
    public void OnDropAmmo(DropAmmoArgs ev) => ev.IsAllowed = false;
}
