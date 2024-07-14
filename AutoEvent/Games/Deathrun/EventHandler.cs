using MEC;
using PlayerRoles;
using InventorySystem.Configs;
using System.Collections.Generic;
using CustomPlayerEffects;
using System.Linq;
using AutoEvent.Events.EventArgs;
using MER.Lite.Components;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using PluginAPI.Core;
using PlayerStatsSystem;
using UnityEngine;

namespace AutoEvent.Games.Deathrun;
public class EventHandler
{
    Plugin _plugin;
    public EventHandler(Plugin plugin)
    {
        _plugin = plugin;
    }

    // If the player presses the Scp-018
    public void OnPickUpItem(PickUpItemArgs ev) // Need rewrite
    {
        if (ev.Item.ItemTypeId != ItemType.SCP018)
            return;
        
        GameObject parent = _plugin.MapInfo.Map.AttachedBlocks.
            First(r => r.gameObject == ev.Pickup.transform.parent.gameObject);
            
        var animation = parent.GetComponentInChildren<Animator>();
        string animationName = animation.runtimeAnimatorController.animationClips.
            First(r => r.name.Contains("Animation")).name;
        animation.Play(animationName);
        
        Timing.CallDelayed(0.2f, () =>
        {
            ev.Player.RemoveItems(ItemType.SCP018);
        });
    }

    public void OnTeamRespawn(TeamRespawnArgs ev) => ev.IsAllowed = false;
    public void OnPlaceBullet(PlaceBulletArgs ev) => ev.IsAllowed = false;
    public void OnPlaceBlood(PlaceBloodArgs ev) => ev.IsAllowed = false;
    public void OnDropItem(DropItemArgs ev) => ev.IsAllowed = false;
    public void OnDropAmmo(DropAmmoArgs ev) => ev.IsAllowed = false;
}
