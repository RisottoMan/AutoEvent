// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         EventHandlers.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/28/2023 1:51 AM
//    Created Date:     10/28/2023 1:51 AM
// -----------------------------------------

using AutoEvent.Events.EventArgs;

namespace AutoEvent.Games.GhostBusters;

public class EventHandler
{
    private Plugin _plugin { get; set; }
    public EventHandler(Plugin plugin)
    {
        _plugin = plugin;
    }
    public void OnTeamRespawn(TeamRespawnArgs ev) => ev.IsAllowed = false;
    public void OnSpawnRagdoll(SpawnRagdollArgs ev) => ev.IsAllowed = false;
    public void OnPlaceBullet(PlaceBulletArgs ev) => ev.IsAllowed = false;
    public void OnPlaceBlood(PlaceBloodArgs ev) => ev.IsAllowed = false;
    public void OnDropItem(DropItemArgs ev) => ev.IsAllowed = false;
    public void OnDropAmmo(DropAmmoArgs ev) => ev.IsAllowed = false;
}