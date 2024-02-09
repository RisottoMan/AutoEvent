using AutoEvent.Events.EventArgs;
using MEC;
using PlayerRoles;
using PlayerStatsSystem;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using System.Collections.Generic;
using UnityEngine;

namespace AutoEvent.Games.Light;
public class EventHandler
{
    private Plugin _plugin;
    public EventHandler(Plugin plugin)
    {
        _plugin = plugin;
    }

    public void OnDamage(PlayerDamageArgs ev)
    {
        if (ev.AttackerHandler is ExplosionDamageHandler explosionDamageHandler)
        {
            explosionDamageHandler.Damage = 0;
        }
    }

    public void OnPlayerNoclip(PlayerNoclipArgs ev)
    {
        ev.IsAllowed = false;

        Transform transform = ev.Player.Camera.transform;
        var ray = new Ray(transform.position + (transform.forward * 0.1f), transform.forward);

        if (!Physics.Raycast(ray, out RaycastHit hit, 1.7f))
            return;

        Player target = Player.Get(hit.collider.transform.root.gameObject);
        if (target == null || ev.Player == target)
            return;

        if (!_plugin.PushCooldown.ContainsKey(ev.Player))
            _plugin.PushCooldown.Add(ev.Player, 0);

        if (_plugin.PushCooldown[ev.Player] > 0)
            return;

        _plugin.PushCooldown[ev.Player] = 3;
        Timing.RunCoroutine(PushPlayer(ev.Player, target));
    }

    private IEnumerator<float> PushPlayer(Player player, Player target)
    {
        Vector3 pushed = player.Camera.transform.forward * 1.7f;
        Vector3 endPos = target.Position + new Vector3(pushed.x, 0, pushed.z);
        int layerAsLayerMask = 0;

        for (int x = 1; x < 8; x++)
            layerAsLayerMask |= (1 << x);

        for (int i = 1; i < 15; i++)
        {
            float movementAmount = 1.7f / 15;
            Vector3 newPos = Vector3.MoveTowards(target.Position, endPos, movementAmount);

            if (Physics.Linecast(target.Position, newPos, layerAsLayerMask))
                yield break;

            target.Position = newPos;
            yield return Timing.WaitForOneFrame;
        }
    }

    [PluginEvent(ServerEventType.PlayerJoined)]
    public void OnPlayerJoin(PlayerJoinedEvent ev)
    {
        ev.Player.SetRole(RoleTypeId.Spectator);
    }

    public void OnTeamRespawn(TeamRespawnArgs ev) => ev.IsAllowed = false;
    public void OnPlaceBullet(PlaceBulletArgs ev) => ev.IsAllowed = false;
    public void OnPlaceBlood(PlaceBloodArgs ev) => ev.IsAllowed = false;
    public void OnDropItem(DropItemArgs ev) => ev.IsAllowed = false;
    public void OnDropAmmo(DropAmmoArgs ev) => ev.IsAllowed = false;
}
