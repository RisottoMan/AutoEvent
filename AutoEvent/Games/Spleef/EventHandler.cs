using AutoEvent.Events.EventArgs;
using InventorySystem.Items.Armor;
using InventorySystem.Items.Firearms;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using UnityEngine;

namespace AutoEvent.Games.Spleef;

public class EventHandler
{
    private Plugin _plugin { get; set; }
    public EventHandler(Plugin plugin)
    {
        _plugin = plugin;
    }
    
    [PluginEvent(ServerEventType.PlayerShotWeapon)]
    public void PlayerShoot(PlayerShotWeaponEvent ev)
    {
        if (!Physics.Raycast(ev.Player.Camera.position, ev.Player.Camera.forward, out RaycastHit raycastHit, 10f, 1 << 0))
        {
            return;
        }
        
        if (_plugin.Config.PlatformHealth < 0)
        {
            return;
        }

        if (ev.Player.CurrentItem is not Firearm firearm)
        {
            return;
        }

        raycastHit.collider.transform.GetComponentsInParent<FallPlatformComponent>().ForEach(GameObject.Destroy);
    }
    
    public void OnTeamRespawn(TeamRespawnArgs ev) => ev.IsAllowed = false;
    public void OnSpawnRagdoll(SpawnRagdollArgs ev) => ev.IsAllowed = false;
    public void OnPlaceBullet(PlaceBulletArgs ev) => ev.IsAllowed = false;
    public void OnPlaceBlood(PlaceBloodArgs ev) => ev.IsAllowed = false;
    public void OnDropItem(DropItemArgs ev) => ev.IsAllowed = false;
    public void OnDropAmmo(DropAmmoArgs ev) => ev.IsAllowed = false;
}