using AutoEvent.Events.EventArgs;
using PlayerRoles;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;

namespace AutoEvent.Games.BuckshotRoulette;
public class EventHandler
{
    private Plugin _plugin;
    public EventHandler(Plugin plugin)
    {
        _plugin = plugin;
    }

    public void OnPickupItem(PickUpItemArgs ev)
    {
        ev.IsAllowed = false;

        if (ev.Player != _plugin.Choser)
            return;

        if (_plugin.EventState != EventState.Playing)
            return;

        if (ev.Item.ItemTypeId == ItemType.SCP018)
        {
            _plugin.GunState = ShotgunState.ShootEnemy;
        }
        else
        {
            _plugin.GunState = ShotgunState.Suicide;
        }
    }

    [PluginEvent(ServerEventType.PlayerJoined)]
    public void OnPlayerJoin(PlayerJoinedEvent ev) => ev.Player.SetRole(RoleTypeId.Spectator);
    public void OnTeamRespawn(TeamRespawnArgs ev) => ev.IsAllowed = false;
    public void OnSpawnRagdoll(SpawnRagdollArgs ev) => ev.IsAllowed = false;
    public void OnPlaceBullet(PlaceBulletArgs ev) => ev.IsAllowed = false;
    public void OnPlaceBlood(PlaceBloodArgs ev) => ev.IsAllowed = false;
    public void OnDropItem(DropItemArgs ev) => ev.IsAllowed = false;
    public void OnDropAmmo(DropAmmoArgs ev) => ev.IsAllowed = false;
}