using AutoEvent.Events.EventArgs;
using MEC;
using PlayerRoles;
using PlayerStatsSystem;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using System.Linq;
using AutoEvent.API.Enums;
using CustomPlayerEffects;
using PluginAPI.Core;
using InventorySystem.Items;

namespace AutoEvent.Games.HideAndSeek;

public class EventHandler
{
    private Plugin _plugin { get; set; }
    public EventHandler(Plugin ev)
    {
        _plugin = ev;
    }

    public void OnPlayerDamage(PlayerDamageArgs ev)
    {
        if (ev.DamageType == DeathTranslations.Falldown.Id)
        {
            ev.IsAllowed = false;
        }

        if (ev.Target.EffectsManager.GetEffect<SpawnProtected>().IsEnabled)
        {
            ev.IsAllowed = false;
            return;
        }

        if (ev.Attacker != null)
        {
            ev.IsAllowed = true;
            bool isAttackerTagger = ev.Attacker.Items.Any(r => r.ItemTypeId == _plugin.Config.TaggerWeapon);
            bool isTargetTagger = ev.Target.Items.Any(r => r.ItemTypeId == _plugin.Config.TaggerWeapon);
            if (!isAttackerTagger || isTargetTagger)
            {
                ev.IsAllowed = false;
                return;
            }

            MakePlayerNormal(ev.Attacker);
            MakePlayerCatchUp(ev.Target);
        }
    }

    public void MakePlayerNormal(Player player)
    {
        player.EffectsManager.EnableEffect<SpawnProtected>(_plugin.Config.NoTagBackDuration, false);
        player.GiveLoadout(_plugin.Config.PlayerLoadouts, LoadoutFlags.IgnoreItems | LoadoutFlags.IgnoreWeapons | LoadoutFlags.IgnoreGodMode);
        player.ClearInventory();
    }

    public void MakePlayerCatchUp(Player player)
    {
        bool isLast = Player.GetPlayers().Count(ply => ply.HasLoadout(_plugin.Config.PlayerLoadouts)) <= _plugin.Config.PlayersRequiredForBreachScannerEffect;
        if (isLast)
            player.GiveEffect(StatusEffect.Scanned, 255, 0f, false);

        player.GiveLoadout(_plugin.Config.TaggerLoadouts, LoadoutFlags.IgnoreItems | LoadoutFlags.IgnoreWeapons | LoadoutFlags.IgnoreGodMode);
        player.ClearInventory();

        ItemBase weapon = player.AddItem(_plugin.Config.TaggerWeapon);
        if (isLast)
            player.GiveEffect(StatusEffect.Scanned, 0, 1f, false);

        Timing.CallDelayed(0.1f, () =>
        {
            player.CurrentItem = weapon;
        });
    }

    [PluginEvent(ServerEventType.PlayerJoined)]
    public void OnJoin(PlayerJoinedEvent ev)
    {
        ev.Player.SetRole(RoleTypeId.Spectator);
    }

    public void OnTeamRespawn(TeamRespawnArgs ev) => ev.IsAllowed = false;
    public void OnSpawnRagdoll(SpawnRagdollArgs ev) => ev.IsAllowed = false;
    public void OnPlaceBullet(PlaceBulletArgs ev) => ev.IsAllowed = false;
    public void OnPlaceBlood(PlaceBloodArgs ev) => ev.IsAllowed = false;
    public void OnDropItem(DropItemArgs ev) => ev.IsAllowed = false;
    public void OnDropAmmo(DropAmmoArgs ev) => ev.IsAllowed = false;
}
