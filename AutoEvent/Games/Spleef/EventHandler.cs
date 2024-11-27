using AutoEvent.Events.EventArgs;
using InventorySystem.Items.Armor;
using InventorySystem.Items.Firearms;
using UnityEngine;

namespace AutoEvent.Games.Spleef;

public class EventHandler
{
    private Plugin _plugin { get; set; }
    public EventHandler(Plugin plugin)
    {
        _plugin = plugin;
    }
    public void OnShot(NewShotEventArgs ev)
    {
        if (_plugin.Config.PlatformHealth < 0)
        {
            return;
        }

        if (ev.Player.CurrentItem is not Firearm firearm)
        {
            return;
        }

        if (ev.Damage <= 0)
        {
            ev.Damage = BodyArmorUtils.ProcessDamage(0, firearm.BaseStats.DamageAtDistance(firearm, ev.Distance), 
                Mathf.RoundToInt(firearm.ArmorPenetration * 100f));
        }

        ev.RaycastHit.collider.transform.GetComponentsInParent<FallPlatformComponent>().ForEach(GameObject.Destroy);
    }
    
    public void OnTeamRespawn(TeamRespawnArgs ev) => ev.IsAllowed = false;
    public void OnSpawnRagdoll(SpawnRagdollArgs ev) => ev.IsAllowed = false;
    public void OnPlaceBullet(PlaceBulletArgs ev) => ev.IsAllowed = false;
    public void OnPlaceBlood(PlaceBloodArgs ev) => ev.IsAllowed = false;
    public void OnDropItem(DropItemArgs ev) => ev.IsAllowed = false;
    public void OnDropAmmo(DropAmmoArgs ev) => ev.IsAllowed = false;
}